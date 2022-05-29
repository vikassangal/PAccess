using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Builder;
using Extensions.UI.Winforms;
using PatientAccess.Actions;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.BillingViews;
using PatientAccess.UI.ClinicalViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.ContactViews;
using PatientAccess.UI.DemographicsViews;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.FinancialCounselingViews;
using PatientAccess.UI.FinancialCounselingViews.PaymentViews;
using PatientAccess.UI.GuarantorViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PhysicianSearchViews;
using PatientAccess.UI.PreRegistrationViews;
using PatientAccess.UI.QuickAccountCreation.ViewImpl;
using PatientAccess.UI.Registration;
using PatientAccess.UI.RegulatoryViews.ViewImpl;
using PatientAccess.UI.ShortRegistration;
using PatientAccess.UI.WorklistViews;
using log4net;
using OnBillingForm = PatientAccess.Rules.OnBillingForm;
using OnClinicalForm = PatientAccess.Rules.OnClinicalForm;
using OnContactsForm = PatientAccess.Rules.OnContactsForm;
using OnDiagnosisForm = PatientAccess.Rules.OnDiagnosisForm;
using OnEmploymentForm = PatientAccess.Rules.OnEmploymentForm;
using OnGuarantorForm = PatientAccess.Rules.OnGuarantorForm;
using OnInsuranceForm = PatientAccess.Rules.OnInsuranceForm;
using OnRegulatoryForm = PatientAccess.Rules.OnRegulatoryForm;
using System.Diagnostics;

namespace PatientAccess.UI
{
    [Serializable]
    [UsedImplicitly]
    public class AccountView : LoggingControlView, IAccountView
    {
        #region Events

        public event EventHandler OnRepeatActivity;
        public event EventHandler OnEditAccount;
        public event EventHandler OnCloseActivity;
        private ICOBReceivedAndIMFMReceivedFeatureManager cobReceivedAndIMFMReceivedFeatureManager;
        #endregion

        #region Event Handlers

        /// <summary>
        /// responsePollTimer_Tick - handler for ResponsePollTimer expired event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void benefitsResponsePollTimer_Tick( object sender, EventArgs e )
        {
            if ( Model_Account == null || Model_Account.Insurance == null )
            {
                StopBenefitsResponsePollTimer();
                return;
            }

            bool blnCheckForPrimary = false;
            bool blnCheckForSecondary = false;

            // see if we've got responses for all coverages that have a data validation ticket with no response

            try
            {
                Coverage primaryCoverage =
                    Model_Account.Insurance.CoverageFor( CoverageOrder.NewPrimaryCoverageOrder() );

                if ( primaryCoverage != null )
                {
                    if (
                        primaryCoverage.DataValidationTicket != null
                        && primaryCoverage.DataValidationTicket.TicketId != string.Empty
                        && !primaryCoverage.DataValidationTicket.ResultsAvailable )
                    {
                        blnCheckForPrimary = true;
                    }
                    else
                    {
                        // update the DV Ticket so the rules will run correctly
                        Model_Account.Insurance.CoverageFor( CoverageOrder.NewPrimaryCoverageOrder() ).
                            DataValidationTicket = primaryCoverage.DataValidationTicket;
                    }
                }

                Coverage secondaryCoverage =
                    Model_Account.Insurance.CoverageFor( CoverageOrder.NewSecondaryCoverageOrder() );

                if ( secondaryCoverage != null )
                {
                    if (
                        secondaryCoverage.DataValidationTicket != null
                        && secondaryCoverage.DataValidationTicket.TicketId != string.Empty
                        && !secondaryCoverage.DataValidationTicket.ResultsAvailable )
                    {
                        blnCheckForSecondary = true;
                    }
                    else
                    {
                        // update the DV Ticket so the rules will run correctly
                        Model_Account.Insurance.CoverageFor( CoverageOrder.NewSecondaryCoverageOrder() ).
                            DataValidationTicket = secondaryCoverage.DataValidationTicket;
                    }
                }

                if ( blnCheckForPrimary )
                {
                    BenefitsValidationResponse response = GetValidationResponse( primaryCoverage );

                    if ( response != null
                        && response.PayorMessage != null
                        && response.PayorMessage.Trim().Length > 0 )
                    {
                        blnCheckForPrimary = false;
                        primaryCoverage.DataValidationTicket.ResultsAvailable = true;

                        btnRefreshToDoList_Click( this, null );

                        if ( !blnCheckForSecondary )
                        {
                            StopBenefitsResponsePollTimer();
                        }
                    }
                }

                if ( blnCheckForSecondary )
                {
                    BenefitsValidationResponse response = GetValidationResponse( secondaryCoverage );

                    if ( response != null
                        && response.PayorMessage != null
                        && response.PayorMessage.Trim().Length > 0 )
                    {
                        blnCheckForSecondary = false;

                        secondaryCoverage.DataValidationTicket.ResultsAvailable = true;

                        btnRefreshToDoList_Click( this, null );

                        if ( !blnCheckForPrimary )
                        {
                            StopBenefitsResponsePollTimer();
                        }
                    }
                }

                if ( !blnCheckForPrimary && !blnCheckForSecondary )
                {
                    btnRefreshToDoList_Click( this, null );
                    StopBenefitsResponsePollTimer();
                }
            }
            catch ( Exception )
            {
                StopBenefitsResponsePollTimer();
                throw;
            }
        }

        private void AccountView_Leave( object sender, EventArgs e )
        {
            // cancel both background workers on laeving the view
            CancelBackgroundWorkers();
        }

        private void AccountView_Load( object sender, EventArgs e )
        {
            tcViewTabPages.SelectedIndex = -1;
        }

        /// <summary>
        /// Fired from InsuranceView when the MSP Wizard does a button click.
        /// Wizard raises event on parent form, FinancialClassesView which raises
        /// this event to display the desired tab page.
        /// </summary>
        private void SetTabPageEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = e as LooseArgs;
            int selectedScreenIndex = ( int )args.Context;

            switch ( ( ScreenIndexes )selectedScreenIndex )
            {
                case ScreenIndexes.INSURED:
                    DisplayInsuredDialogPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

                case ScreenIndexes.SECONDARYINSURED:
                    DisplayInsuredDialogPage( CoverageOrder.NewSecondaryCoverageOrder() );
                    break;

                case ScreenIndexes.PAYORDETAILS:
                    DisplayPayorDetailsDialogPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

                case ScreenIndexes.SECONDARYPAYORDETAILS:
                    DisplayPayorDetailsDialogPage( CoverageOrder.NewSecondaryCoverageOrder() );
                    break;

                case ScreenIndexes.PRIMARYINSVERIFICATION:
                    DisplayInsuranceVerificationPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

                case ScreenIndexes.SECONDARYINSVERIFICATION:
                    DisplayInsuranceVerificationPage( CoverageOrder.NewSecondaryCoverageOrder() );
                    break;

                case ScreenIndexes.PRIMARYAUTHORIZATION:
                    DisplayInsuranceAuthorizationPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

                case ScreenIndexes.SECONDARYAUTHORIZATION:
                    DisplayInsuranceAuthorizationPage( CoverageOrder.NewSecondaryCoverageOrder() );
                    break;
                default:
                    tcViewTabPages.SelectedIndex = selectedScreenIndex;
                    break;
            }
        }

        /// <summary>
        /// Fired from RequiredFieldsSummaryView when NPI details of a NonStaff Physician is not completed.
        /// </summary>
        private void SetNonStaffPhysicianTabPageEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            int selectedScreenIndex = ( int )args.Context;

            if ( !IsNonStaffPhysicianScreenIndex( selectedScreenIndex ) ) return;

            Cursor = Cursors.WaitCursor;

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.CLINICAL;
            PhysicianSearchFormView physicianSearchForm =
                PhysicianSearchFormView.GetPhysicianSearchForm( ( ScreenIndexes )selectedScreenIndex, Model_Account );

            physicianSearchForm.UpdateView();
            physicianSearchForm.ShowDialog( this );
            Cursor = Cursors.Default;

            clinicalView1.Model = Model_Account;
            clinicalView1.UpdateView();
        }

        public static bool IsNonStaffPhysicianScreenIndex( int selectedScreenIndex )
        {
            return ( ( ( ScreenIndexes )selectedScreenIndex == ScreenIndexes.REFERRINGNONSTAFFPHYSICIAN ) ||
                     ( ( ScreenIndexes )selectedScreenIndex == ScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN ) ||
                     ( ( ScreenIndexes )selectedScreenIndex == ScreenIndexes.ATTENDINGNONSTAFFPHYSICIAN ) ||
                     ( ( ScreenIndexes )selectedScreenIndex == ScreenIndexes.OPERATINGNONSTAFFPHYSICIAN ) ||
                     ( ( ScreenIndexes )selectedScreenIndex == ScreenIndexes.PRIMARYCARENONSTAFFPHYSICIAN )
                   );
        }

        private void demographicsView_RefreshTopPanel( object sender, EventArgs e )
        {
            patientContextView.GenderLabelText = String.Empty;
            patientContextView.PatientNameText = String.Empty;

            if ( ( ( ( LooseArgs )e ).Context ) != null )
            {
                if ( ( ( Patient )
                     ( ( ( LooseArgs )e ).Context ) ).Sex.Description != String.Empty &&
                    ( ( Patient )
                     ( ( ( LooseArgs )e ).Context ) ).Sex.Description != String.Empty )
                {
                    patientContextView.GenderLabelText = ( ( Patient )
                                                               ( ( ( LooseArgs )e ).Context ) ).Sex.Description;
                }

                patientContextView.PatientNameText = ( ( Patient )
                                                           ( ( ( LooseArgs )e ).Context ) ).Name.AsFormattedName();

                patientContextView.DateOfBirthText = ( ( Patient )
                                                           ( ( ( LooseArgs )e ).Context ) ).DateOfBirth.Date.ToString
                    ( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );

                patientContextView.SocialSecurityNumber = ( ( Patient )
                                                                ( ( ( LooseArgs )e ).Context ) ).SocialSecurityNumber.
                    AsFormattedString();

                if ( patientContextView.DateOfBirthText == DateTime.MinValue.ToString( "MM/dd/yyyy" ) )
                {
                    patientContextView.DateOfBirthText = String.Empty;
                }
            }
        }

        private void NoPrimaryMedicarePayorForAutoAccidentEventHandler( object sender, EventArgs e )
        {
            MessageDisplayHandler = new ErrorMessageDisplayHandler( Model as Account );
            DialogResult warningResult =
                MessageDisplayHandler.DisplayYesNoErrorMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );
            if ( warningResult == DialogResult.No )
            {
                return;
            }
            DoNotProceedWithFinish = true;
            ReEnableFinishButtonAndFusIcon();
            EnableInsuranceTabHandler( this, new LooseArgs( Model_Account ) );
        }
        private void EnableInsuranceTabHandler( object sender, EventArgs e )
        {
            const int index = ( int )ScreenIndexes.INSURANCE;

            SetActiveTab( index );
            insuranceView.Focus();
            btnBack.Enabled = btnNext.Enabled = true;
            EnableInsuranceTab = true;
        }

        private void tcViewTabPages_SelectedIndexChanged( object sender, EventArgs e )
        {
            processTab();

            // TO DO :
            // find a way to keep focus when the arrow left, right keys are being used to navigate the tabs

            if ( lastKeyPressed == RIGHTARROW || lastKeyPressed == LEFTARROW )
            {
                tcViewTabPages.Focus();
            }
        }

        private void btnRefreshToDoList_Click( object sender, EventArgs e )
        {
            PopulateToDolistOnNewThread( false );
        }

        private void btnCancel_Click( object sender, EventArgs e )
        {
            if ( !checkForError() )
            {
                DialogResult result = MessageBox.Show(
                    UIErrorMessages.INCOMPLETE_ACTIVITY_MSG,
                    UIErrorMessages.ACTIVITY_DIALOG_TITLE, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation );

                if ( result == DialogResult.Yes )
                {
                    // Close VIWeb external browser
                    CloseVIweb();
                    // Add code to cancel background workers in a private method 
                    // that can be called from the appropriate places
                    CancelBackgroundWorkers();

                    // Close Account Supplemental Information View if open for Online PreRegistration Account creation
                    ViewFactory.Instance.CreateView<PatientAccessView>().CloseAccountSupplementalInformationView();

                    if ( c_singletonInstance != null && c_singletonInstance.WasInvokedFromWorklistItem )
                    {
                        // This view created from worklist. Signal parent form to close this view's
                        // parent view.  The Worklist view remains loaded.
                        c_singletonInstance.WasInvokedFromWorklistItem = false;
                        maintenanceCmdView = MaintenanceCmdView.GetInstance();
                        WorklistCmdAggregator aggregator =
                            WorklistCmdAggregator.GetInstance();

                        aggregator.RaiseActionSelectedEvent( maintenanceCmdView, new LooseArgs( this ) );
                        ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent(
                            sender, new LooseArgs( Model_Account ) );

                        //Remove this line after worklistview occurrence code issues.
                        ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( sender, e );
                    }
                    else
                    {
                        ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent(
                            sender, new LooseArgs( Model_Account ) );
                        ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( sender, e );
                    }
                    //SR1557 release lock for mother's account for Admit/Pre-Admit Newborn
                    if (Model_Account != null && (Model_Account.Activity.IsPreAdmitNewbornActivity() || 
                            (Model_Account.Activity.IsAdmitNewbornActivity() && Model_Account.Activity.AssociatedActivityType==null)) )
                    {
                        ReleaseLockOnMotherAccount();
                    }
                    Dispose();
                }
                else
                {
                    btnCancel.Enabled = true;
                }
            }
        }
        // Code for close browser if user want's to come out from an
        // activity
        public static void CloseVIweb()
        {
            if (VIwebHTML5Handler.PID.Count > 0)
            {
                try
                {
                    if (VIwebHTML5Handler.BrowerType == "Chrome")
                    {
                        Process[] processes = Process.GetProcessesByName("chrome");
                        foreach (var pid in processes)
                        {
                            if (pid.MainWindowTitle.Contains("Scan Document") || pid.MainWindowTitle.Contains("View Document"))
                            {
                                //Avoid closing of whole chrome browser opened tab
                                pid.CloseMainWindow();
                            }
                        }
                        processes = Process.GetProcessesByName("chrome");
                        foreach (var pid in processes)
                        {
                            if (pid.MainWindowTitle.Contains("Scan Document") || pid.MainWindowTitle.Contains("View Document"))
                            {
                                pid.Kill();
                            }
                        }
                    }
                    if (VIwebHTML5Handler.BrowerType == "Edge")
                    {
                        var process = Process.GetProcessesByName("msedge");
                        if (process.Length > 0)
                        {
                            foreach (var proc in process)
                            {
                                if (proc.MainWindowTitle.Contains("Scan Document") || proc.MainWindowTitle.Contains("View Document"))
                                {
                                    //Avoid closing of whole Edge browser opened tab
                                    proc.CloseMainWindow();
                                }
                              
                            }
                        }
                        process = Process.GetProcessesByName("msedge");
                        if (process.Length > 0)
                        {
                            foreach (var proc in process)
                            {
                                if (proc.MainWindowTitle.Contains("Scan Document") || proc.MainWindowTitle.Contains("View Document"))
                                {
                                    proc.Kill();
                                }
                            }
                        }
                    }

                    if(VIwebHTML5Handler.BrowerType=="Firefox")
                    {
                       var process = Process.GetProcessesByName("firefox");
                        if(process.Length>0)
                        {
                            foreach (var proc in process)
                            {
                                if (proc.MainWindowTitle.Contains("Scan Document") || proc.MainWindowTitle.Contains("View Document"))
                                {
                                    proc.Kill();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    
                }                
            }
        }

        /// <summary>
        /// Cancel both background workers that are part of this class.
        /// Problem: if the background worker is buys this will cancel it regardless.
        /// how likely is it that both workers will be active at the same time.
        /// </summary>
        private void CancelBackgroundWorkers()
        {
            // cancel the background worker(s) here...
            if ( null != backgroundWorker )
                backgroundWorker.CancelAsync();

            if ( null != toDoListWorker )
                toDoListWorker.CancelAsync();
        }

        /// <summary>
        /// When the BACK button is clicked, make the prior tab page active.        
        /// </summary>
        private void btnBack_Click( object sender, EventArgs e )
        {
            if ( !checkForError() )
            {
                if ( tcViewTabPages.SelectedIndex > 0 )
                {
                    tcViewTabPages.SelectedIndex = tcViewTabPages.SelectedIndex - 1;
                }
            }
        }

        /// <summary>
        /// When the NEXT button is clicked, make the next tab page active.
        /// </summary>
        private void btnNext_Click( object sender, EventArgs e )
        {
            // Do not process Next click if Insurance tab has to be enabled for patient age over 65 
            // check, since otherwise it is getting redirected to Guarantor tab instead of Insurance tab.
            if ( EnableInsuranceTab )
            {
                EnableInsuranceTab = false;
                return;
            }

            if ( !checkForError() )
            {
                if ( tcViewTabPages.SelectedIndex != tcViewTabPages.TabCount )
                {
                    tcViewTabPages.SelectedIndex = tcViewTabPages.SelectedIndex + 1;
                }
            }
        }

        private void btnFinish_Click( object sender, EventArgs e )
        {
            DoNotProceedWithFinish = false;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );

            if ( !MedicareOver65Checked )
            {
                if ( CheckMedicareOver65() )
                {
                    ReEnableFinishButtonAndFusIcon();
                    return;
                }
            }

            //TODO-AC we need to improve this design.
            EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( NoPrimaryMedicarePayorForAutoAccidentEventHandler );
            if ( DoNotProceedWithFinish )
            {
                return;
            }
            EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage();
            if ( DoNotProceedWithFinish )
            {
                ReEnableFinishButtonAndFusIcon();
                return;
            }
            //TODO-AC end todo

            Cursor = Cursors.WaitCursor;

            SetActivatingTab( string.Empty );
            bool isPhysicianValid = PhysiciansValidated();

            if ( isPhysicianValid != true )
            {
                ReEnableFinishButtonAndFusIcon();
                return;
            }
            if (Model_Account.BillHasDropped)
            {
                Model_Account.BalanceDue = this.Model_Account.TotalCurrentAmtDue - Model_Account.TotalPaid;
            }
            if ( !RuleEngine.EvaluateAllRules( Model_Account ) )
            {
                string inValidCodesSummary = RuleEngine.GetInvalidCodeFieldSummary();

                if ( inValidCodesSummary != string.Empty )
                {
                    invalidCodeFieldsDialog = new InvalidCodeFieldsDialog();
                    invalidCodeOptionalFieldsDialog = new InvalidCodeOptionalFieldsDialog();

                    if ( Model_Account.DischargeDate == DateTime.MinValue )
                    {
                        invalidCodeFieldsDialog.ErrorText = inValidCodesSummary;
                        invalidCodeFieldsDialog.UpdateView();

                        try
                        {
                            invalidCodeFieldsDialog.ShowDialog( this );
                            Cursor = Cursors.Default;
                            RuleEngine.ClearActions();
                            runRulesForTab();
                        }
                        finally
                        {
                            invalidCodeFieldsDialog.Dispose();
                            invalidCodeOptionalFieldsDialog.Dispose();
                        }
                        ReEnableFinishButtonAndFusIcon();
                        return;
                    }

                    invalidCodeOptionalFieldsDialog.ErrorText = inValidCodesSummary;
                    invalidCodeOptionalFieldsDialog.UpdateView();
                    try
                    {
                        invalidCodeOptionalFieldsDialog.ShowDialog( this );
                        if ( invalidCodeOptionalFieldsDialog.DialogResult.ToString() != "Yes" )
                        {
                            RuleEngine.ClearActions();
                            runRulesForTab();
                            ReEnableFinishButtonAndFusIcon();
                            return;
                        }
                    }
                    finally
                    {
                        invalidCodeOptionalFieldsDialog.Dispose();
                        invalidCodeFieldsDialog.Dispose();
                    }
                }

                string summary = RuleEngine.GetRemainingErrorsSummary();

                if ( summary != String.Empty )
                {
                    requiredFieldsDialog = new RequiredFieldsDialog();
                    requiredFieldsDialog.Title = "Warning for Remaining Errors";
                    requiredFieldsDialog.HeaderText = "The fields listed have values that are creating errors. Visit each field\r\n"
                                                           +
                                                           "listed and correct the value before completing this activity.";
                    requiredFieldsDialog.ErrorText = summary;
                    try
                    {
                        requiredFieldsDialog.ShowDialog( this );
                        Cursor = Cursors.Default;
                        RuleEngine.ClearActions();
                        runRulesForTab();
                    }
                    finally
                    {
                        requiredFieldsDialog.Dispose();
                    }
                    ReEnableFinishButtonAndFusIcon();
                    return;
                }

                ICollection<CompositeAction> collection = RuleEngine.GetCompositeItemsCollection();

                if ( collection.Count > 0 &&
                     Rules.RuleEngine.AccountHasRequiredFields( collection ) )
                {
                    Cursor = Cursors.WaitCursor;
                    RequiredFieldSummaryView = new RequiredFieldsSummaryView();
                    RequiredFieldsSummaryPresenter = new RequiredFieldsSummaryPresenter( RequiredFieldSummaryView, collection )
                                                     {
                                                         Model = Model_Account,
                                                         Header = CommonControls.RequiredFieldsSummaryPresenter.REQUIRED_FIELDS_HEADER
                                                     };

                    registerTabSelectedEvent();
                    RegisterNonStaffPhysicianTabSelectedEvent();
                    RegisterRequiredFieldSummaryToInsuranceView();

                    RequiredFieldsSummaryPresenter.ShowViewAsDialog( this );
                    RuleEngine.ClearActions();
                    runRulesForTab();
                    ReEnableFinishButtonAndFusIcon();

                    return;
                }
            }

            // If patient or financial class is changed and user clicks on 'Finish' button,
            // following 2 method calls will ensure that COB Received and IMFM Received field value will be reset if not applicable.
            COBReceivedIMFMReceivedFeatureManager.IfApplicableResetCOBReceivedOn(Model_Account);
            COBReceivedIMFMReceivedFeatureManager.IfApplicableResetIMFMReceivedOn(Model_Account);

            // BUG 1512 Fix - Verify if SpanCodes 70 and 71 are still valid for
            // the current Patient Type, in case the Patient Type was changed,
            // and 'Finish' clicked, without going to the Billing View.

            Patient patient = Model_Account.Patient;
            patient.SelectedAccount = Model_Account;

            if ( Model_Account != null &&
                 !Model_Account.Activity.GetType().Equals( typeof( EditAccountActivity ) ) &&
                 !Model_Account.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) )
            {
                patient.ClearPriorSystemGeneratedOccurrenceSpans();

                SpanCode spanCode70 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
                SpanCode spanCode71 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.PRIOR_STAY_DATES );

                patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );
            }

            // BUG 1512 Fix - End

            //Raise Activitycomplete event
            if ( !checkForError() && ValidateHSVPlanFinancialClassMapping() )
            {
                if ( !CheckForDuplicatePreRegAccounts() )
                {
                    RuleEngine.ClearActions();
                    runRulesForTab();
                    ReEnableFinishButtonAndFusIcon();
                    return;
                }
                lvToDo.Enabled = false;
                btnRefreshToDoList.Enabled = false;
                btnCancel.Enabled = false;
                btnBack.Enabled = false;
                btnNext.Enabled = false;
                SaveAccount();
            }
            else
            {
                ReEnableFinishButtonAndFusIcon();
                return;
            }

            RuleEngine.UnloadHandlers();
            //SR1557 release lock for mother's account for Admit/Pre-Admit Newborn
            if ( Model_Account.Activity.IsPreAdmitNewbornActivity() ||
                    (Model_Account.Activity.IsAdmitNewbornActivity() && Model_Account.Activity.AssociatedActivityType == null) )
            {
                ReleaseLockOnMotherAccount();
            }
            // Close Account Supplemental Information View if open for Online PreRegistration Account creation
            ViewFactory.Instance.CreateView<PatientAccessView>().CloseAccountSupplementalInformationView();

            Cursor = Cursors.Default;
            // Close VIweb browser after finishing the activity
            CloseVIweb();
        }
        
        private void ReleaseLockOnMotherAccount()
        {
            if ( Model_Account!= null && Model_Account.Patient.MothersAccount != null && 
                    AccountLockStatus.IsAccountLocked( Model_Account.Patient.MothersAccount, User.GetCurrent() ) )
            {
                AccountActivityService.ReleaseAccountlock( Model_Account.Patient.MothersAccount );
            }
        }

        private void EvaluateMedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage()
        {
            bool ruleWasViolated = !RuleEngine.OneShotRuleEvaluation<MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage>(
                               Model_Account, MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler );

            DoNotProceedWithFinish = ruleWasViolated;
        }

        private void MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler( object sender, EventArgs e )
        {
            MessageDisplayHandler = new ErrorMessageDisplayHandler( Model as Account );
            MessageDisplayHandler.DisplayOkWarningMessageFor(
                typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ) );
        }

        internal void EvaluateNoPrimaryMedicareForAutoAccidentRuleOnFinish( EventHandler eventHandler )
        {
            bool noPrimaryMedicareForAutoAccidentRuleChecked = MessageStateManager.
                HasErrorMessageBeenDisplayedEarlierFor(
                typeof( NoMedicarePrimaryPayorForAutoAccident ).ToString() );
            if ( !noPrimaryMedicareForAutoAccidentRuleChecked )
            {
                RuleEngine.OneShotRuleEvaluation<NoMedicarePrimaryPayorForAutoAccident>( Model, eventHandler );
            }
        }

        private void requiredFieldSummary_TabSelectedEvent( object sender, EventArgs e )
        {
            int index = ( int )( ( LooseArgs )e ).Context;

            // only process those on this view (tab indexes 0 - 11)

            if ( index < 12 )
            {
                tcViewTabPages.SelectedIndex = index;
                tcViewTabPages.TabPages[index].Select();
                processTab();
            }
        }

        private void lvToDo_SelectedIndexChanged( object sender, EventArgs e )
        {
            string selectedItemDesc = string.Empty;

            ListView lv = sender as ListView;
            if ( initialDisplay )
            {
                initialDisplay = false;
            }
            if ( lv != null && lv.SelectedItems.Count > 0 && lv.SelectedItems[0].Tag != null )
            {
                if ( lv.SelectedItems[0].Tag is CompositeAction )
                {
                    CompositeAction ca = ( CompositeAction )lv.SelectedItems[0].Tag;
                    selectedToDoItemOid = ca.Oid;
                    selectedItemDesc = ca.Description;
                }
                else if ( lv.SelectedItems[0].Tag is LeafAction )
                {
                    LeafAction la = ( LeafAction )lv.SelectedItems[0].Tag;
                    selectedToDoItemOid = la.Oid;
                    selectedItemDesc = la.Description;
                }
            }

            if ( selectedItemDesc != string.Empty )
            {
                BreadCrumbLogger.GetInstance.Log( String.Format( "Selected TODO item :{0}",
                                                               selectedItemDesc ) );
            }
        }

        /// <summary>
        /// Save the selected item in the ToDo list so the list can be redrawn with the
        /// last selected item preserved.
        /// </summary>

        private void registerTabSelectedEvent()
        {
            EventHandler eventHandler = requiredFieldSummary_TabSelectedEvent;

            if ( !RequiredFieldsSummaryPresenter.IsTabEventRegistered( eventHandler ) )
            {
                RequiredFieldsSummaryPresenter.TabSelectedEvent += eventHandler;
            }
        }

        private void RegisterNonStaffPhysicianTabSelectedEvent()
        {
            EventHandler eventHandler = SetNonStaffPhysicianTabPageEventHandler;

            if ( !RequiredFieldsSummaryPresenter.IsTabEventRegistered( eventHandler ) )
            {
                RequiredFieldsSummaryPresenter.TabSelectedEvent += eventHandler;
            }
        }

        /// <summary>
        /// On tab focus to the ListView, set first item to the selected state.
        /// (Windows Forms doesn't do this the same way a mouse click does.)
        /// </summary>
        private void ToDoListView_Enter( object sender, EventArgs e )
        {
            ListView lv = sender as ListView;
            if ( initialDisplay )
            {
                // This handler is invoked when form is painted.  Requirements
                // dictate that no action item is selected on first display
                return;
            }

            if ( lv == null ) return;

            if ( lv.Items.Count == 0 )
            {
                return;
            }

            if (lv.SelectedItems.Count != 0)
            {
                lv.Items[0].Selected = true;
            }

            if (lv.SelectedItems.Count > 0 && lv.SelectedItems[0].Tag != null)
            {
                if (lv.SelectedItems[0].Tag is CompositeAction)
                {
                    CompositeAction ca = (CompositeAction) lv.SelectedItems[0].Tag;
                    selectedToDoItemOid = ca.Oid;
                }
                else if (lv.SelectedItems[0].Tag is LeafAction)
                {
                    LeafAction la = (LeafAction) lv.SelectedItems[0].Tag;
                    selectedToDoItemOid = la.Oid;
                }
            }
        }

        /// <summary>
        /// If an item in the ToDo list is selected by the keyboard, set it to the selected state.
        /// (Windows Forms doesn't do this the same way a mouse click does.)
        /// </summary>
        private void ToDoListView_KeyDown( object sender, KeyEventArgs e )
        {
            ListView lv = sender as ListView;
            if ( lv == null ) return;

            if ( lv.SelectedItems.Count > 0 && lv.SelectedItems[0].Tag != null )
            {
                if ( lv.SelectedItems[0].Tag is CompositeAction )
                {
                    CompositeAction ca = ( CompositeAction )lv.SelectedItems[0].Tag;
                    selectedToDoItemOid = ca.Oid;
                }
                else if ( lv.SelectedItems[0].Tag is LeafAction )
                {
                    LeafAction la = ( LeafAction )lv.SelectedItems[0].Tag;
                    selectedToDoItemOid = la.Oid;
                }
            }
        }

        /// <summary>
        /// registerConfirmationView1_EditAccount - fire the edit account event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerConfirmationView1_EditAccount( object sender, EventArgs e )
        {
            if ( e != null )
            {
                IAccount selectedAccount = ( ( LooseArgs )e ).Context as IAccount;

                if ( Model != null )
                {
                    OnEditAccount( this, new LooseArgs( selectedAccount ) );
                }
            }
        }

        /// <summary>
        /// registerConfirmationView1_CloseActivity - fire the close activity event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerConfirmationView1_CloseActivity( object sender, EventArgs e )
        {
            if ( OnCloseActivity != null )
            {
                // cancel the background workers here...
                CancelBackgroundWorkers();

                OnCloseActivity( this, null );
            }
        }

        /// <summary>
        /// registerConfirmationView1_RepeatActivity - fire the repeat activity event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerConfirmationView1_RepeatActivity( object sender, EventArgs e )
        {
            //Raise RepeatActivity event.
            if ( OnRepeatActivity != null )
            {
                OnRepeatActivity( this, new LooseArgs( new Patient() ) );
            }
        }

        private void clinicalView1_FocusOutOfPhysicianSelectArea( object sender, EventArgs e )
        {
            AcceptButton = btnNext;
        }

        private void ErrorMessageDisplayedForRuleEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = e as LooseArgs;
            if ( args != null && args.Context != null )
            {
                string rule = args.Context.ToString();
                if ( rule == null )
                {
                    return;
                }
                MessageStateManager.SetErrorMessageDisplayedFor( rule, true );
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// stopResponsePollTimer - stop the polling timer
        /// </summary>
        public void StopBenefitsResponsePollTimer()
        {
            if ( i_BenefitsResponsePollTimer != null )
            {
                i_BenefitsResponsePollTimer.Stop();
                i_BenefitsResponsePollTimer.Enabled = false;
            }
        }

        /// <summary>
        /// startResponsePollTimer - start the timer to poll every few (currently 10) seconds
        /// </summary>
        public void StartBenefitsResponsePollTimer()
        {
            i_BenefitsResponsePollTimer.Tick -= benefitsResponsePollTimer_Tick;
            i_BenefitsResponsePollTimer.Start();
            i_BenefitsResponsePollTimer.Enabled = true;
            i_BenefitsResponsePollTimer.Interval = RESPONSE_POLL_INTERVAL;
            i_BenefitsResponsePollTimer.Tick += benefitsResponsePollTimer_Tick;
        }

        public void ShowPanel()
        {
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            progressPanel1.Visible = false;
            progressPanel1.SendToBack();
        }

        public void SetActivatingTab( string value )
        {
            ActivatingTab = value;
        }

        public void DisplayLocationRequiredFieldSummary()
        {
            ICollection<CompositeAction> collection = RuleEngine.GetCompositeItemsCollection();
            if ( collection.Count > 0
                && Rules.RuleEngine.AccountHasRequiredFields( collection ) )
            {
                RequiredFieldsSummaryView requiredFieldsSummaryView = new RequiredFieldsSummaryView();
                RequiredFieldsSummaryPresenter requiredFieldsSummaryPresenter = new RequiredFieldsSummaryPresenter( requiredFieldsSummaryView, "Warning - Fields Required for Bed Assignment" );
                Cursor = Cursors.WaitCursor;
                requiredFieldsSummaryPresenter.SetActionItems( collection );
                requiredFieldsSummaryPresenter.Header = "In order for the system to ensure that a duplicate bed assignment is not "
                                                        +
                                                        "inadvertently made for a patient, the following required fields must be completed before the location selection may continue. "
                                                        +
                                                        "Double-click a row in the table to complete the required field or click OK to dismiss this message.";
                EventHandler eventHandler = requiredFieldSummary_TabSelectedEvent;
                requiredFieldsSummaryPresenter.TabSelectedEvent += eventHandler;
                requiredFieldsSummaryPresenter.ShowViewAsDialog( this );

                RuleEngine.ClearActions();
                Cursor = Cursors.Default;
                return;
            }
        }

        public bool IsMedicareAdvisedForPatient()
        {
            if ( Model_Account != null && Model_Account.Patient != null )
            {
                if ( !MedicareOver65Checked )
                {

                    Patient patient = Model_Account.Patient;
                    ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                    int gmtOffset = patient.Facility.GMTOffset;
                    int dstOffset = patient.Facility.DSTOffset;

                    int patientAge = patient.AgeInYearsFor(timeBroker.TimeAt(gmtOffset, dstOffset));
                    Coverage primaryCoverage = Model_Account.Insurance.PrimaryCoverage;
                    Coverage secondaryCoverage = Model_Account.Insurance.SecondaryCoverage;

                    // Display message if user selects a non-Medicare primary payor where the patient 
                    // is over 65 and the secondary payor is null or not entered.
                    if ( ( patientAge == AGE_SIXTY_FIVE || patientAge > AGE_SIXTY_FIVE )
                        && ( primaryCoverage != null && primaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) )
                        && ( secondaryCoverage == null
                             || secondaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) ) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ActivateEmploymentTab()
        {
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }
            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.EMPLOYMENT;
            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();
            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void SetModel( Account anAccount )
        {
            Model = anAccount;

            //Raise activity start event to PatientAccessView
            //Added for worklist MaintenanceActivity - 
            //Model was set after calling the AccountView.GetInstance() in all Actions.Execute()

            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( c_singletonInstance, EventArgs.Empty );
        }

        public Account GetModel()
        {
            return Model_Account;
        }

        public void ActivateAccount()
        {
            // raise the ActivatePreregisteredAccountEvent

            SearchEventAggregator aggregator =
                SearchEventAggregator.GetInstance();

            aggregator.RaiseActivatePreregisteredAccountEvent( this, new LooseArgs( Model_Account ) );
        }

        public void ActivateDemographicsTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.DEMOGRAPHICS;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );

            UpdateView();
        }

        public void ActivateDiagnosisTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.DIAGNOSIS;


            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateInsuranceTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.TabIndex = 0;
            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.INSURANCE;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateGuarantorTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.GUARANTOR;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateCounselingTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.COUNSELING;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivatePaymentTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.PAYMENT;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateContactsTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.CONTACTS;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateClinicalTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.CLINICAL;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateRegulatoryTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.REGULATORY;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public void ActivateDocumentsTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new MaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.DOCUMENTS;

            PatientAccessViewPopulationAggregator aggregator =
                PatientAccessViewPopulationAggregator.GetInstance();

            aggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );
            UpdateView();
        }

        public override void UpdateView()
        {
            FinancialCouncelingService.PriorAccountsRetrieved = false;
            UpdateView2();
            ViewFactory.Instance.CreateView<PatientAccessView>().Model = Model_Account;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions( Model_Account );
        }
        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties
        public Account Model_Account
        {
            get { return ( Account )Model; }
        }
        public bool WasInvokedFromWorklistItem
        {
            private get { return wasInvokedFromWorklistItem; }
            set { wasInvokedFromWorklistItem = value; }
        }
        public string ActiveContext
        {
            set { userContextView.Description = value; }
        }
        public BackgroundWorker SavingAccountBackgroundWorker
        {
            get { return backgroundWorker; }
        }
        internal IMessageDisplayStateManager MessageStateManager { get; private set; }
        private IErrorMessageDisplayHandler MessageDisplayHandler { get; set; }
        internal IRuleEngine RuleEngine { get; private set; }
        private string ActivatingTab { get; set; }
        private bool DoNotProceedWithFinish { get; set; }
        public bool Over65Check { private get; set; }
        public bool MedicareOver65Checked { get; set; }
        public bool EnableInsuranceTab { private get; set; }
        public IRequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter { get; private set; }

        public static bool IsShortRegAccount
        {
            get { return i_IsShortRegAccount; }
            set { i_IsShortRegAccount = value; }
        }

        public static bool IsNewbornAccount
        {
            get; set;
		}
        public static bool IsQuickRegistered { private get; set; }
        public static bool IsPAIWalkinRegistered { private get; set; }

        private ICOBReceivedAndIMFMReceivedFeatureManager COBReceivedIMFMReceivedFeatureManager
        {
            get
            {
                if ( cobReceivedAndIMFMReceivedFeatureManager == null )
                {
                    cobReceivedAndIMFMReceivedFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
                }

                return cobReceivedAndIMFMReceivedFeatureManager;
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Private Methods
        private void RegisterRequiredFieldSummaryToInsuranceView()
        {
            insuranceView.RegisterRequiredFieldSummaryTabSelectedEvent();
        }
        private void SetActiveTab()
        {
            int index;

            SuspendLayout();

            switch ( ActivatingTab )
            {
                case "Billing":
                    {
                        index = ( int )ScreenIndexes.BILLING;
                        break;
                    }
                case "Clinical":
                    {
                        index = ( int )ScreenIndexes.CLINICAL;
                        break;
                    }
                case "Contacts":
                    {
                        index = ( int )ScreenIndexes.CONTACTS;
                        break;
                    }
                case "Counseling":
                    {
                        index = ( int )ScreenIndexes.COUNSELING;
                        break;
                    }
                case "Demographics":
                    {
                        index = ( int )ScreenIndexes.DEMOGRAPHICS;
                        break;
                    }
                case "Diagnosis":
                    {
                        index = ( int )ScreenIndexes.DIAGNOSIS;
                        break;
                    }
                case "Documents":
                    {
                        index = ( int )ScreenIndexes.DOCUMENTS;
                        break;
                    }
                case "Employment":
                    {
                        index = ( int )ScreenIndexes.EMPLOYMENT;
                        break;
                    }
                case "Guarantor":
                    {
                        index = ( int )ScreenIndexes.GUARANTOR;
                        break;
                    }
                case "Insurance":
                    {
                        index = ( int )ScreenIndexes.INSURANCE;
                        break;
                    }
                case "Insured":
                    {
                        index = ( int )ScreenIndexes.INSURED;
                        break;
                    }
                case "SecondaryInsured":
                    {
                        index = ( int )ScreenIndexes.SECONDARYINSURED;
                        break;
                    }
                case "Payor":
                    {
                        index = ( int )ScreenIndexes.PAYORDETAILS;
                        break;
                    }
                case "SecondaryPayor":
                    {
                        index = ( int )ScreenIndexes.SECONDARYPAYORDETAILS;
                        break;
                    }
                case "Payment":
                    {
                        index = ( int )ScreenIndexes.PAYMENT;
                        break;
                    }
                case "Regulatory":
                    {
                        index = ( int )ScreenIndexes.REGULATORY;
                        break;
                    }
                case "PrimaryInsuranceVerification":
                    {
                        index = ( int )ScreenIndexes.PRIMARYINSVERIFICATION;
                        break;
                    }
                case "SecondaryInsuranceVerification":
                    {
                        index = ( int )ScreenIndexes.SECONDARYINSVERIFICATION;
                        break;
                    }
                case "PrimaryAuthorization":
                    {
                        index = ( int )ScreenIndexes.PRIMARYAUTHORIZATION;
                        break;
                    }
                case "SecondaryAuthorization":
                    {
                        index = ( int )ScreenIndexes.SECONDARYAUTHORIZATION;
                        break;
                    }
                default:
                    index = ( int )ScreenIndexes.DEMOGRAPHICS;
                    break;
            }

            SetTabPageEventHandler( this, new LooseArgs( index ) );
        }

        private void SetActiveTab( int selectedScreenIndex )
        {
            tcViewTabPages.TabPages[selectedScreenIndex].Select();

            if ( tcViewTabPages.SelectedIndex != selectedScreenIndex )
            {
                tcViewTabPages.SelectedIndex = selectedScreenIndex;
            }
        }

        private void BeforeSaveAccount()
        {
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterSaveAccount( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            progressPanel1.Visible = false;
            progressPanel1.SendToBack();

            if ( e.Cancelled )
            {
                lvToDo.Enabled = true;
                btnRefreshToDoList.Enabled = true;
                btnCancel.Enabled = true;
                btnBack.Enabled = ( tcViewTabPages.SelectedTab != tpDemographics );
                btnNext.Enabled = ( tcViewTabPages.SelectedTab != tpDocuments );
                btnFinish.Enabled = true;
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success
                Model_Account.Patient.SetPatientContextHeaderData();
                ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model_Account ) );
                DisplayConfirmationScreen();
                patientContextView.Model = Model_Account.Patient;
                patientContextView.Account = Model_Account;
                patientContextView.UpdateView();

                // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
            }

            Cursor = Cursors.Default;
        }


        private void DoSaveAccount( object sender, DoWorkEventArgs e )
        {
            Model_Account.Facility = User.GetCurrent().Facility;

            Account anAccount = Model_Account;
            Activity currentActivity = Model_Account.Activity;
            currentActivity.AppUser = User.GetCurrent();
            CoverageDefaults coverageDefaults = new CoverageDefaults();
            coverageDefaults.SetCoverageDefaultsForActivity( anAccount );

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            if ( broker != null )
            {
                AccountSaveResults results = broker.Save( anAccount, currentActivity );
                results.SetResultsTo( Model_Account );
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// CheckForDuplicatePreRegAccounts
        /// </summary>
        private bool CheckForDuplicatePreRegAccounts()
        {
            bool result = true;
            if ( Model_Account.KindOfVisit != null &&
                Model_Account.KindOfVisit.Code == VisitType.PREREG_PATIENT )
            {
                var searchCriteria = new DuplicatePreRegAccountsSearchCriteria(
                    Model_Account.Facility.Oid,
                    Model_Account.AccountNumber,
                    Model_Account.Patient.Name,
                    Model_Account.Patient.SocialSecurityNumber,
                    Model_Account.Patient.DateOfBirth,
                    Model_Account.Patient.MedicalRecordNumber,
                    Model_Account.AdmitDate );

                var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

                accountProxiesCollection = accountBroker.SelectDuplicatePreRegAccounts( searchCriteria );

                if ( accountProxiesCollection != null && accountProxiesCollection.Count > 0 )
                {
                    using ( showDuplicatePreRegAccountsDialog = new ShowDuplicatePreRegAccountsDialog() )
                    {
                        showDuplicatePreRegAccountsDialog.Model = accountProxiesCollection;
                        showDuplicatePreRegAccountsDialog.UpdateView();
                        showDuplicatePreRegAccountsDialog.ShowDialog();
                        if ( showDuplicatePreRegAccountsDialog.DialogResult == DialogResult.No )
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Save Account
        /// </summary>
        private void SaveAccount()
        {

            if ( backgroundWorker == null
                ||
                ( backgroundWorker != null
                 && !backgroundWorker.IsBusy )
                )
            {
                BeforeSaveAccount();

                backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerSupportsCancellation = true;

                backgroundWorker.DoWork += DoSaveAccount;
                backgroundWorker.RunWorkerCompleted += AfterSaveAccount;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void ReEnableFinishButtonAndFusIcon()
        {
            Cursor = Cursors.Default;
            btnFinish.Enabled = true;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( true );
        }

        private bool ValidateHSVPlanFinancialClassMapping()
        {
            bool isValid = true;
            if ( Model_Account.Activity.IsSubjectToHSVPlanFinancialClassRestriction )
            {
                bool isValidMappingForPrimary = false;
                Insurance insurance = Model_Account.Insurance;
                FinancialClass financialClass = Model_Account.FinancialClass;
                HospitalService hospitalService = Model_Account.HospitalService;
                if ( insurance != null && financialClass != null && hospitalService != null )
                {
                    if ( financialClass.IsSpecialtyMedicare() )
                    {
                        Coverage primaryCoverage = insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
                        if ( primaryCoverage != null && primaryCoverage.InsurancePlan != null )
                        {
                            isValidMappingForPrimary = IsValidHSVPlanForSpecialtyMedicare( hospitalService.Code,
                                                                                          primaryCoverage.InsurancePlan.
                                                                                              PlanID );
                        }
                        if ( !isValidMappingForPrimary )
                        {
                            Coverage secondaryCoverage = insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
                            if ( secondaryCoverage == null || secondaryCoverage.InsurancePlan == null )
                            {
                                isValid = false;
                                MessageBox.Show( UIErrorMessages.HSV_PLAN_PRIMARY_VALIDATION_MSG, "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                                MessageBoxDefaultButton.Button1 );
                            }
                            else
                            {
                                bool isValidMappingForSecondary = IsValidHSVPlanForSpecialtyMedicare( hospitalService.Code,
                                                                                                     secondaryCoverage.
                                                                                                         InsurancePlan.PlanID );
                                if ( !isValidMappingForSecondary )
                                {
                                    isValid = false;
                                    MessageBox.Show( UIErrorMessages.HSV_PLAN_VALIDATION_MSG, "Error",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                                    MessageBoxDefaultButton.Button1 );
                                }
                            }
                        }
                    }
                }
            }
            return isValid;
        }

        private static bool IsValidHSVPlanForSpecialtyMedicare( string hsvCode, string planId )
        {
            IInsuranceBroker insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
            bool isValidMapping = insuranceBroker.IsValidHSVPlanForSpecialtyMedicare( User.GetCurrent().Facility.Oid, hsvCode,
                                                                                planId );
            return isValidMapping;
        }


        private static BenefitsValidationResponse GetValidationResponse( Coverage aCoverage )
        {
            IDataValidationBroker iBroker =
                BrokerFactory.BrokerOfType<IDataValidationBroker>();

            BenefitsValidationResponse benefitsValidationResponse = iBroker.GetBenefitsValidationResponse(
                aCoverage.DataValidationTicket.TicketId,
                User.GetCurrent().SecurityUser.UPN,
                aCoverage.GetType() );

            return benefitsValidationResponse;
        }

        private bool checkForError()
        {

            bool rcErrors = RuleEngine.AccountHasFailedError();

            return rcErrors;
        }

        private bool CheckForAgeOver65Error()
        {

            bool rcErrors = RuleEngine.AccountHasFailedError();

            if ( !rcErrors )
            {
                if ( Over65Check && tcViewTabPages.SelectedIndex == ( int )ScreenIndexes.INSURANCE )
                {
                    rcErrors = true;
                }
            }

            return rcErrors;
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if ( ( keyData == Keys.Right ) && ( tcViewTabPages.Focused ) )
            {
                lastKeyPressed = RIGHTARROW;

                tcViewTabPages_SelectedIndexChanged( this, null );
            }
            else if ( ( keyData == Keys.Left ) && ( tcViewTabPages.Focused ) )
            {
                lastKeyPressed = LEFTARROW;
                if ( tcViewTabPages.Focused )
                {
                    tcViewTabPages_SelectedIndexChanged( this, null );
                }
            }
            else
            {
                lastKeyPressed = string.Empty;
            }

            return base.ProcessCmdKey( ref msg, keyData );
        }

        protected override void WndProc( ref Message m )
        {
            const uint WM_NOTIFY = 0x004E;
            const uint TCN_FIRST = 0xFFFFFDDA;
            const uint TCN_SELCHANGING = TCN_FIRST - 2;

            base.WndProc( ref m );

            switch ( ( uint )m.Msg )
            {
                case WM_NOTIFY:
                    {

                        if ( progressPanel1.Visible )
                        {
                            int loading = 1;
                            m.Result = ( IntPtr )loading;
                            return;
                        }

                        int idCtrl = ( int )m.WParam;

                        NMHDR nmh = new NMHDR();
                        nmh.hwndFrom = IntPtr.Zero;
                        nmh.idFrom = 0;
                        nmh.code = 0;

                        nmh = ( NMHDR )m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            if ( !checkForError() )
                            {
                                Validate();
                            }

                            bool rc = CheckForAgeOver65Error();
                            int irc = 0;
                            if ( rc )
                            {
                                Over65Check = false;
                                irc = 1;
                            }

                            m.Result = ( IntPtr )irc;
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>
        /// displayConfirmationScreen - display a confirmation screen if the activate was successful
        /// </summary>
        private void DisplayConfirmationScreen()
        {
            panel1.Hide();
            panelToDoList.Hide();
            tcViewTabPages.Hide();

            registerConfirmationView1.Model = Model_Account;
            HidePanel();
            registerConfirmationView1.UpdateView();
            panelConfirmation.Show();
        }

        private void lvToDo_DoubleClick( object sender, EventArgs e )
        {
            if ( lvToDo.SelectedItems.Count > 0 && lvToDo.SelectedItems[0].Tag != null )
            {
                CompositeAction action = lvToDo.SelectedItems[0].Tag as CompositeAction;

                if ( action == null )
                {
                    IAction leaf = lvToDo.SelectedItems[0].Tag as IAction;
                    if ( leaf != null )
                    {
                        Cursor = Cursors.WaitCursor;
                        leaf.Execute();
                        SetActivatingTab( leaf.Context.ToString() );
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    Cursor = Cursors.WaitCursor;
                    action.Execute();
                    SetActivatingTab( action.Context.ToString() );
                    Cursor = Cursors.Default;
                }

                SetActiveTab();
            }
        }

        /// <summary>
        /// The MSP Wizard dialog has a button to display the InsuranceDetails dialog's
        /// Insured tabPage.  This is called from SetTabPageEventHandler.
        /// </summary>
        private void DisplayInsuranceVerificationPage( CoverageOrder coverageOrder )
        {
            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.INSURANCE;

            InsuranceDetails insuranceDetails = new InsuranceDetails
            {
                insuranceDetailsView =
                    {
                        Model_Coverage = (coverageOrder.Oid == CoverageOrder.PRIMARY_OID)
                                ? Model_Account.Insurance.GetPrimaryCoverage()
                                : Model_Account.Insurance.GetSecondaryCoverage(),
                        Account = Model_Account,
                        Active_Tab = InsuranceDetailsView.VERIFICATION_DETAILS_PAGE
                    }
            };

            insuranceDetails.UpdateView();
            insuranceDetails.ShowDialog( this );
        }

        /// <summary>
        /// The MSP Wizard dialog has a button to display the InsuranceDetails dialog's
        /// Insured tabPage.  This is called from SetTabPageEventHandler.
        /// </summary>
        private void DisplayInsuredDialogPage( CoverageOrder coverageOrder )
        {
            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.INSURANCE;

            InsuranceDetails insuranceDetails = new InsuranceDetails
            {
                insuranceDetailsView =
                    {
                        Model_Coverage = ( coverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                                ? Model_Account.Insurance.GetPrimaryCoverage()
                                : Model_Account.Insurance.GetSecondaryCoverage(),
                        Account = Model_Account,
                        Active_Tab = InsuranceDetailsView.INSURED_DETAILS_PAGE
                    }
            };

            insuranceDetails.UpdateView();
            insuranceDetails.ShowDialog( this );
        }

        /// <summary>
        /// The MSP Wizard dialog has a button to display the InsuranceDetails dialog's
        /// Payor Details tabPage.  This is called from SetTabPageEventHandler.
        /// </summary>
        private void DisplayPayorDetailsDialogPage( CoverageOrder coverageOrder )
        {
            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.INSURANCE;

            InsuranceDetails insuranceDetails = new InsuranceDetails
            {
                insuranceDetailsView =
                    {
                        Model_Coverage = ( coverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                                ? Model_Account.Insurance.GetPrimaryCoverage()
                                : Model_Account.Insurance.GetSecondaryCoverage(),
                        Account = Model_Account,
                        Active_Tab = InsuranceDetailsView.PLAN_DETAILS_PAGE
                    }
            };

            insuranceDetails.UpdateView();
            insuranceDetails.ShowDialog( this );
        }

        /// <summary>
        /// Display Insurance Authorization Page
        /// </summary>
        /// <param name="coverageOrder"></param>
        private void DisplayInsuranceAuthorizationPage( CoverageOrder coverageOrder )
        {
            tcViewTabPages.SelectedIndex = ( int )ScreenIndexes.INSURANCE;

            InsuranceDetails insuranceDetails = new InsuranceDetails
            {
                insuranceDetailsView =
                    {
                        Model_Coverage = ( coverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                                ? Model_Account.Insurance.GetPrimaryCoverage()
                                : Model_Account.Insurance.GetSecondaryCoverage(),
                        Account = Model_Account,
                        Active_Tab = InsuranceDetailsView.INSURANCE_AUTHORIZATION_PAGE
                    }
            };

            insuranceDetails.UpdateView();
            insuranceDetails.ShowDialog( this );


            // TODO: Add error log message here
            // Iterate over the coverage collection and populate
            // the primary and secondary insurance screens.
        }

        private void DoPopulateToDolist( object sender, DoWorkEventArgs e )
        {
            // not the most elegant or efficient way to be polling CancellationPending
            // but sort of achieves the functionality we desire
            if ( toDoListWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            PopulateToDolist();

            if ( toDoListWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void PopulateToDolistOnNewThread( bool useNewThread )
        {
            Cursor = Cursors.AppStarting;
            lvToDo.Items.Clear();
            lvToDo.BeginUpdate();

            if ( useNewThread )
            {
                if ( toDoListWorker == null
                    ||
                    ( toDoListWorker != null
                     && !toDoListWorker.IsBusy )
                    )
                {
                    toDoListWorker = new BackgroundWorker();
                    toDoListWorker.WorkerSupportsCancellation = true;

                    toDoListWorker.DoWork += DoPopulateToDolist;
                    toDoListWorker.RunWorkerCompleted += AfterPopulateToDoList;

                    toDoListWorker.RunWorkerAsync();
                }
            }
            else
            {
                try
                {
                    PopulateToDolist();
                }
                catch ( RemotingTimeoutException )
                {
                    if ( lvToDo != null && !lvToDo.IsDisposed && !lvToDo.Disposing )
                    {
                        lvToDo.EndUpdate();
                        lvToDo.Refresh();
                        MessageBox.Show( UIErrorMessages.TIMEOUT_GENERAL );
                    }
                }

                AfterPopulatingToDoList();
            }

            Cursor = Cursors.Default;
        }

        private void AfterPopulatingToDoList()
        {
            if ( lvToDo != null && !lvToDo.IsDisposed && !lvToDo.Disposing )
            {
                if ( worklistitems != null
                    && worklistitems.Count > 0 )
                {
                    foreach ( ListViewItem lvi in worklistitems )
                    {
                        if (!lvToDo.Items.Contains(lvi))
                        {
                            lvToDo.Items.Add(lvi);
                        }
                    }
                }

                lvToDo.TabStop = lvToDo.Items.Count > 0;

                lvToDo.EndUpdate();
                lvToDo.Refresh();
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterPopulateToDoList( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if ( e.Error.GetType() == typeof( RemotingTimeoutException ) )
                {
                    if ( lvToDo != null && !lvToDo.IsDisposed && !lvToDo.Disposing )
                    {
                        lvToDo.EndUpdate();
                        lvToDo.Refresh();
                        MessageBox.Show( UIErrorMessages.TIMEOUT_GENERAL );
                    }
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                AfterPopulatingToDoList();
            }
        }

        private void PopulateToDolist()
        {
            string kindOfVisitCode = string.Empty;
            string financialClassCode = string.Empty;

            RuleBrokerProxy brokerProxy = new RuleBrokerProxy();
            IWorklistSettingsBroker wb = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();

            if ( Model_Account.KindOfVisit != null )
            {
                kindOfVisitCode = Model_Account.KindOfVisit.Code;
            }
            if ( Model_Account.FinancialClass != null )
            {
                financialClassCode = Model_Account.FinancialClass.Code;
            }

            Hashtable actionWorklistMapping = brokerProxy.ActionWorklistMapping( kindOfVisitCode, financialClassCode );
            ArrayList worklists = wb.GetAllWorkLists();
            worklistitems = new ArrayList();


            // run the rules!
            if ( Model_Account != null )
            {
                ArrayList anArray = RuleEngine.GetWorklistActionItems( Model_Account );
                anArray.Sort();

                // break out if the bg worker is cancelling

                if ( toDoListWorker != null
                    && toDoListWorker.CancellationPending )
                {
                    return;
                }

                if ( anArray != null && anArray.Count > 0 )
                {
                    foreach ( LeafAction la in anArray )
                    {
                        // break out if the bg worker is cancelling

                        if ( toDoListWorker != null
                            && toDoListWorker.CancellationPending )
                        {
                            return;
                        }

                        ArrayList al = ( ArrayList )actionWorklistMapping[la.Oid];

                        string worklistName;
                        if ( al != null )
                        {
                            worklistName = ( string )al[1];
                        }
                        else
                        {
                            if ( kindOfVisitCode == VisitType.PREREG_PATIENT )
                            {
                                worklistName = worklists[( int )Worklist.PREREGWORKLISTID - 1] as string;
                            }
                            else if ( kindOfVisitCode == VisitType.EMERGENCY_PATIENT )
                            {
                                worklistName = worklists[( int )Worklist.EMERGENCYDEPARMENTWORKLISTID - 1] as string;
                            }
                            else
                            {
                                worklistName = worklists[( int )Worklist.POSTREGWORKLISTID - 1] as string;
                            }
                        }

                        if ( la.IsComposite )
                        {
                            CompositeAction ca = ( CompositeAction )la;

                            if ( ca != null )
                            {
                                ListViewItem lvi = new ListViewItem();
                                lvi.Tag = ca;
                                lvi.Text = ca.Description;
                                lvi.SubItems.Add( ca.NumberOfAllLeafActions().ToString() );
                                lvi.SubItems.Add( worklistName );
                                worklistitems.Add( lvi );
                            }
                        }
                        else
                        {
                            // only 'concrete' actions that are not composited get added to the list;
                            // generic actions not composited do not

                            if ( la.GetType() != typeof( GenericAction ) )
                            {
                                ListViewItem lvi = new ListViewItem { Tag = la, Text = la.Description };
                                lvi.SubItems.Add( "1" );
                                lvi.SubItems.Add( worklistName );
                                worklistitems.Add( lvi );
                            }
                        }
                    }
                }
            }
        }

        private void BeginUpdateForAllControlsIn( ControlCollection aControlsCollection )
        {
            foreach ( Control c in aControlsCollection )
            {
                ComboBox cb = c as ComboBox;
                if ( cb != null )
                {
                    cb.BeginUpdate();
                    c_log.DebugFormat( "Setting BeginUpdate for {0}", cb.Name );
                }
                BeginUpdateForAllControlsIn( c.Controls );
            }
        }

        private void EndUpdateForAllControlsIn( ControlCollection aControlsCollection )
        {
            foreach ( Control c in aControlsCollection )
            {
                ComboBox cb = c as ComboBox;
                if ( cb != null )
                {
                    cb.EndUpdate();
                    c_log.DebugFormat( "Setting EndUpdate for {0}", cb.Name );
                }
                EndUpdateForAllControlsIn( c.Controls );
            }
        }

        private void processTab()
        {
            try
            {
                Cursor = Cursors.AppStarting;
                BeginUpdateForAllControlsIn( Controls );

                if ( tcViewTabPages.SelectedTab != null )
                {
                    BreadCrumbLogger.GetInstance.Log( tcViewTabPages.SelectedTab.Text +
                                                     " tab selected", Model_Account );
                }

                if ( tcViewTabPages.SelectedTab == tpDemographics )
                {
                    demographicsView.Model = Model_Account;
                    demographicsView.UpdateView();
                    demographicsView.Focus();
                    btnBack.Enabled = false;
                    btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpEmployment )
                {
                    patientEmploymentView.Model = Model_Account;
                    patientEmploymentView.UpdateView();
                    patientEmploymentView.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpDiagnosis )
                {
                    diagnosisView.Model = Model_Account;
                    diagnosisView.UpdateView();
                    diagnosisView.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpInsurance )
                {
                    insuranceView.Model = Model_Account;
                    insuranceView.UpdateView();
                    insuranceView.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;

                    // OTD# 35691 :- When creating a new account from an existing account where patient Age >= 65 yrs
                    // and primary & secondary insurance are not Medicare, the Insurance tab appears blank after
                    // clicking 'Yes' on the 'Patient is 65 or older' message from demographics tab 'Next' click.
                    //
                    // Fix :- Since 'insuranceView.Visible == false' in the above scenario, call 'Show()'
                    // explicitly on the SelectedTab and UpdateView to enable the relevant fields.
                    if ( !insuranceView.Visible )
                    {
                        if (tcViewTabPages.SelectedTab != null) tcViewTabPages.SelectedTab.Show();
                        insuranceView.UpdateView();
                        EnableInsuranceTab = false;
                    }
                }
                else if ( tcViewTabPages.SelectedTab == tpGuarantor )
                {
                    guarantorView.Model = Model_Account;
                    guarantorView.UpdateView();
                    guarantorView.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpBilling )
                {
                    if ( billingView != null )
                    {
                        billingView.Model = Model_Account;
                        billingView.UpdateView();
                        billingView.Focus();
                    }
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpCounseling )
                {
                    controlViewForLiability = FinancialCounselingViewFactory.LiabilityViewFor( Model_Account );
                    tpCounseling.Controls.Clear();
                    tpCounseling.Controls.Add( controlViewForLiability );

                    if ( controlViewForLiability.GetType() == typeof( UnInsuredFinancialCounselingView ) )
                    {
                        ( ( UnInsuredFinancialCounselingView )controlViewForLiability ).EnableInsuranceTab -=
                            EnableInsuranceTabHandler;
                        ( ( UnInsuredFinancialCounselingView )controlViewForLiability ).EnableInsuranceTab +=
                            EnableInsuranceTabHandler;
                    }
                    if ( controlViewForLiability.GetType() == typeof( InsuredFinancialCounselingView ) )
                    {
                        ( ( InsuredFinancialCounselingView )controlViewForLiability ).EnableInsuranceTab -=
                            EnableInsuranceTabHandler;
                        ( ( InsuredFinancialCounselingView )controlViewForLiability ).EnableInsuranceTab +=
                            EnableInsuranceTabHandler;
                    }
                    if ( controlViewForLiability.GetType() == typeof( IncompleteInsuranceFinancialCounselingView ) )
                    {
                        ( ( IncompleteInsuranceFinancialCounselingView )controlViewForLiability ).EnableInsuranceTab
                            -=
                            EnableInsuranceTabHandler;
                        ( ( IncompleteInsuranceFinancialCounselingView )controlViewForLiability ).EnableInsuranceTab
                            +=
                            EnableInsuranceTabHandler;
                    }

                    controlViewForLiability.Model = Model_Account;
                    controlViewForLiability.UpdateView();
                    controlViewForLiability.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpPayment )
                {
                    controlViewForPayment = FinancialCounselingViewFactory.PaymentViewFor( Model_Account );

                    if ( controlViewForPayment.GetType() == typeof( PaymentView ) )
                    {
                        ( ( PaymentView )controlViewForPayment ).EnableInsuranceTab -=
                            EnableInsuranceTabHandler;
                        ( ( PaymentView )controlViewForPayment ).EnableInsuranceTab +=
                            EnableInsuranceTabHandler;
                    }
                    if ( controlViewForPayment.GetType() == typeof( IncompleteInsuranceFinancialCounselingView ) )
                    {
                        ( ( IncompleteInsuranceFinancialCounselingView )controlViewForPayment ).EnableInsuranceTab -=
                            EnableInsuranceTabHandler;
                        ( ( IncompleteInsuranceFinancialCounselingView )controlViewForPayment ).EnableInsuranceTab +=
                            EnableInsuranceTabHandler;
                    }

                    controlViewForPayment.Model = Model_Account;
                    tpPayment.Controls.Add( controlViewForPayment );
                    controlViewForPayment.UpdateView();
                    controlViewForPayment.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpClinical )
                {
                    clinicalView1.Model = Model_Account;
                    clinicalView1.UpdateView();
                    clinicalView1.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpContacts )
                {
                    patientContactsView.Model = Model_Account;
                    patientContactsView.UpdateView();
                    patientContactsView.Focus();
                    btnBack.Enabled = btnNext.Enabled = true;
                    btnFinish.Enabled = true;
                }
                else if ( tcViewTabPages.SelectedTab == tpRegulatory )
                {
                    if ( !CheckMedicareOver65() )
                    {
                        regulatoryView.Model = Model_Account;
                        regulatoryView.UpdateView();
                        regulatoryView.Focus();
                        btnBack.Enabled = btnNext.Enabled = true;
                        btnFinish.Enabled = true;
                    }
                }
                else if ( tcViewTabPages.SelectedTab == tpDocuments )
                {
                    listOfDocumentsView.Model = Model_Account;
                    listOfDocumentsView.UpdateView();
                    listOfDocumentsView.Focus();
                    btnBack.Enabled = true;
                    btnNext.Enabled = false;
                    btnFinish.Enabled = true;
                }
            }
            finally
            {
                EndUpdateForAllControlsIn( Controls );
                Cursor = Cursors.Default;
            }
        }

        private bool CheckMedicareOver65()
        {
            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if ( IsMedicareAdvisedForPatient() )
            {
                MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                                                             UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                                                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if ( warningResult == DialogResult.Yes )
                {
                    EnableInsuranceTabHandler( this, new LooseArgs( Model_Account ) );
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// UpdateView2 method called from UpdateView.
        /// </summary>
        private void UpdateView2()
        {

            if ( Model_Account != null )
            {
                insuranceView.Model_Account = Model_Account;

                // if this account has a bed, then force retrieval of accomodation codes for
                // quicker loading of the Diagnosis view

                if ( Model_Account.Location != null
                    && Model_Account.Location.NursingStation != null )
                {
                    diagnosisView.NursingStationCode = Model_Account.Location.NursingStation.Code;
                }

                //Remove financial class if it's Insured with no insurance
                if ( i_FinancialCouncelingService == null )
                {
                    i_FinancialCouncelingService = new FinancialCouncelingService();
                    bool isUninsuredFCWithoutInsurance = i_FinancialCouncelingService.IsUninsuredFCWithoutInsurance( Model_Account );
                    if ( !isUninsuredFCWithoutInsurance )
                    {
                        Model_Account.FinancialClass = null;
                    }
                }

                patientContextView.Model = Model_Account.Patient;
                patientContextView.Account = Model_Account;
                patientContextView.UpdateView();

                if ( Model_Account.Activity != null )
                {
                    userContextView.Description = Model_Account.Activity.ContextDescription;
                }

                loadRules();
            }

            SetActiveTab();

            progressPanel1.Visible = false;

            if ( lvToDo.Items.Count <= 0 )
            {
                PopulateToDolistOnNewThread( true );
            }

            // display action items if this is an existing account

            if ( lvToDo.Items.Count > 0 )
            {
                if ( selectedToDoItemOid != -1 )
                {
                    // Item was selected by the user -- find it and set it to selected state
                    bool foundItem = false;
                    ListView.ListViewItemCollection lvic = lvToDo.Items;

                    // Find the item the was the last selected item
                    for ( int index = 0; index < lvic.Count; index++ )
                    {
                        ListViewItem lvi = lvic[index];
                        if ( lvi.Tag == null )
                        {
                            continue;
                        }
                        if ( lvi.Tag is CompositeAction )
                        {
                            CompositeAction cp = lvi.Tag as CompositeAction;

                            if ( cp.Oid == selectedToDoItemOid )
                            {
                                lvToDo.Items[index].Selected = true;
                                foundItem = true;
                                break;
                            }
                        }
                        else if ( lvi.Tag is LeafAction )
                        {
                            LeafAction cp = lvi.Tag as LeafAction;

                            if ( cp.Oid == selectedToDoItemOid )
                            {
                                lvToDo.Items[index].Selected = true;
                                foundItem = true;
                                break;
                            }
                        }
                    }
                    if ( foundItem == false )
                    {
                        // Item is gone; select the first item
                        selectedToDoItemOid = -1;
                        lvToDo.Items[0].Selected = true;
                    }
                }
                else
                {
                    // No item was selected by user, so set first item to selected state
                    lvToDo.Items[0].Selected = initialDisplay == false;
                }

                lvToDo.Invalidate();
                lvToDo.TabStop = true;
            }
            else
            {
                lvToDo.TabStop = false;
            }
            //The below code to avid users clicking on the buton before the account view is loaded completely
            btnRefreshToDoList.Enabled = true;
        }

        private void runRulesForTab()
        {
            if ( tcViewTabPages.SelectedTab == tpDemographics )
            {
                RuleEngine.EvaluateRule( typeof( OnPatientDemographicsForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpEmployment )
            {
                RuleEngine.EvaluateRule( typeof( OnEmploymentForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpDiagnosis )
            {
                RuleEngine.EvaluateRule( typeof( OnDiagnosisForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpInsurance )
            {
                RuleEngine.EvaluateRule( typeof( OnInsuranceForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpGuarantor )
            {
                RuleEngine.EvaluateRule( typeof( OnGuarantorForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpBilling )
            {
                RuleEngine.EvaluateRule( typeof( OnBillingForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpCounseling )
            {
                // ???
            }
            else if ( tcViewTabPages.SelectedTab == tpPayment )
            {
                // ???
            }
            else if ( tcViewTabPages.SelectedTab == tpClinical )
            {
                RuleEngine.EvaluateRule( typeof( OnClinicalForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpContacts )
            {
                RuleEngine.EvaluateRule( typeof( OnContactsForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpRegulatory )
            {
                RuleEngine.EvaluateRule( typeof( OnRegulatoryForm ), Model_Account );
            }
            else if ( tcViewTabPages.SelectedTab == tpDocuments )
            {
                // no rules
            }
        }

        private void loadRules()
        {
            if ( !blnRulesLoaded )
            {
                blnRulesLoaded = true;

                RuleEngine.LoadRules( Model_Account );
            }
        }

        private bool PhysiciansValidated()
        {
            return PhysicianService.VerifyPhysicians( Model_Account,
                                                     GetReferringPhysicianId(), GetAdmittingPhysicianId(),
                                                     GetAttendingPhysicianId(), GetOperatingPhysicianId(),
                                                     GetPrimaryCarePhysicianId() );
        }

        private string GetAdmittingPhysicianId()
        {
            string result = string.Empty;
            if ( Model_Account != null && Model_Account.AdmittingPhysician != null )
            {
                result = Model_Account.AdmittingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetAttendingPhysicianId()
        {
            string result = string.Empty;
            if ( Model_Account != null && Model_Account.AttendingPhysician != null )
            {
                result = Model_Account.AttendingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetOperatingPhysicianId()
        {
            string result = string.Empty;
            if ( Model_Account != null && Model_Account.OperatingPhysician != null )
            {
                result = Model_Account.OperatingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetPrimaryCarePhysicianId()
        {
            string result = string.Empty;
            if ( Model_Account != null && Model_Account.PrimaryCarePhysician != null )
            {
                result = Model_Account.PrimaryCarePhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetReferringPhysicianId()
        {
            string result = string.Empty;

            if ( Model_Account != null && Model_Account.ReferringPhysician != null )
            {
                result = Model_Account.ReferringPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( AccountView ) );
            this.userContextView = new PatientAccess.UI.UserContextView();
            this.patientContextView = new PatientAccess.UI.PatientContextView();
            this.guarantorView = new PatientAccess.UI.GuarantorViews.GuarantorView();
            this.listOfDocumentsView = new PatientAccess.UI.DocumentImagingViews.ListOfDocumentsView();
            this.paymentView = new Extensions.UI.Winforms.ControlView();
            this.controlViewForLiability = new Extensions.UI.Winforms.ControlView();
            this.copyPartyView = new PatientAccess.UI.GuarantorViews.CopyPartyView();
            this.billingView = new PatientAccess.UI.BillingViews.BillingView();
            this.insuranceView = new PatientAccess.UI.InsuranceViews.InsuranceView();
            this.demographicsView = new PatientAccess.UI.DemographicsViews.DemographicsView();
            this.patientContactsView = new PatientAccess.UI.ContactViews.PatientContactsView();
            this.patientEmploymentView = new PatientAccess.UI.DemographicsViews.DemographicsEmploymentView();
            this.registerConfirmationView1 = new PatientAccess.UI.Registration.RegisterConfirmationView();
            this.panelConfirmation = new System.Windows.Forms.Panel();
            this.tcViewTabPages = new System.Windows.Forms.TabControl();
            this.tpDemographics = new System.Windows.Forms.TabPage();
            this.tpEmployment = new System.Windows.Forms.TabPage();
            this.tpDiagnosis = new System.Windows.Forms.TabPage();
            this.tpClinical = new System.Windows.Forms.TabPage();
            this.clinicalView1 = new PatientAccess.UI.ClinicalViews.ClinicalView();
            this.tpInsurance = new System.Windows.Forms.TabPage();
            this.tpGuarantor = new System.Windows.Forms.TabPage();
            this.tpBilling = new System.Windows.Forms.TabPage();
            this.tpCounseling = new System.Windows.Forms.TabPage();
            this.tpPayment = new System.Windows.Forms.TabPage();
            this.tpContacts = new System.Windows.Forms.TabPage();
            this.tpRegulatory = new System.Windows.Forms.TabPage();
            this.pnlRegulatoryView = new System.Windows.Forms.Panel();
            this.regulatoryView = new PatientAccess.UI.RegulatoryViews.ViewImpl.RegulatoryView();
            this.tpDocuments = new System.Windows.Forms.TabPage();
            this.panelToDoList = new System.Windows.Forms.Panel();
            this.lvToDo = new System.Windows.Forms.ListView();
            this.chAction = new System.Windows.Forms.ColumnHeader();
            this.chCount = new System.Windows.Forms.ColumnHeader();
            this.chWorklist = new System.Windows.Forms.ColumnHeader();
            this.lblStaticToDoList = new System.Windows.Forms.Label();
            this.btnRefreshToDoList = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnFinish = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.btnNext = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnBack = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancel = new PatientAccess.UI.CommonControls.ClickOnceLoggingButton();
            this.panelUserContext = new System.Windows.Forms.Panel();
            this.panelConfirmation.SuspendLayout();
            this.tcViewTabPages.SuspendLayout();
            this.tpDemographics.SuspendLayout();
            this.tpEmployment.SuspendLayout();
            this.tpDiagnosis.SuspendLayout();
            this.tpClinical.SuspendLayout();
            this.tpInsurance.SuspendLayout();
            this.tpGuarantor.SuspendLayout();
            this.tpContacts.SuspendLayout();
            this.tpRegulatory.SuspendLayout();
            this.pnlRegulatoryView.SuspendLayout();
            this.tpDocuments.SuspendLayout();
            this.panelToDoList.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelUserContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView
            // 
            this.userContextView.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 94 ) ), ( ( System.Byte )( 137 ) ),
                                                                           ( ( System.Byte )( 185 ) ) );
            this.userContextView.Description = "Preregister Patient";
            this.userContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userContextView.Location = new System.Drawing.Point( 0, 0 );
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new System.Drawing.Size( 1024, 22 );
            this.userContextView.TabIndex = 0;
            this.userContextView.TabStop = false;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size( 1003, 24 );
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;
            // 
            // guarantorView
            // 
            this.guarantorView.BackColor = System.Drawing.Color.White;
            this.guarantorView.Location = new System.Drawing.Point( 0, 0 );
            this.guarantorView.Model = null;
            this.guarantorView.Model_Account = null;
            this.guarantorView.Name = "guarantorView";
            this.guarantorView.Size = new System.Drawing.Size( 1024, 378 );
            this.guarantorView.TabIndex = 0;
            this.guarantorView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // listOfDocumentsView
            // 
            this.listOfDocumentsView.BackColor = System.Drawing.Color.White;
            this.listOfDocumentsView.Location = new System.Drawing.Point( 0, 0 );
            this.listOfDocumentsView.Model = null;
            this.listOfDocumentsView.Name = "listOfDocumentsView";
            this.listOfDocumentsView.Size = new System.Drawing.Size( 1024, 378 );
            this.listOfDocumentsView.TabIndex = 0;
            this.listOfDocumentsView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // paymentView
            // 
            this.paymentView.BackColor = System.Drawing.Color.White;
            this.paymentView.Location = new System.Drawing.Point( 0, 0 );
            this.paymentView.Model = null;
            this.paymentView.Name = "paymentView";
            this.paymentView.TabIndex = 0;
            // 
            // controlViewForLiability
            // 
            this.controlViewForLiability.Location = new System.Drawing.Point( 0, 0 );
            this.controlViewForLiability.Model = null;
            this.controlViewForLiability.Name = "controlViewForLiability";
            this.controlViewForLiability.TabIndex = 0;
            // 
            // copyPartyView
            // 
            this.copyPartyView.CoverageOrder =
                ( ( PatientAccess.Domain.CoverageOrder )( resources.GetObject( "copyPartyView.CoverageOrder" ) ) );
            this.copyPartyView.KindOfTargetParty = null;
            this.copyPartyView.Location = new System.Drawing.Point( 0, 0 );
            this.copyPartyView.Model = null;
            this.copyPartyView.Name = "copyPartyView";
            this.copyPartyView.Size = new System.Drawing.Size( 178, 24 );
            this.copyPartyView.TabIndex = 0;
            // 
            // insuranceView
            // 
            this.insuranceView.BackColor = System.Drawing.Color.White;
            this.insuranceView.Location = new System.Drawing.Point( 0, 0 );
            this.insuranceView.Model = null;
            this.insuranceView.Name = "insuranceView";
            this.insuranceView.Size = new System.Drawing.Size( 1024, 378 );
            this.insuranceView.TabIndex = 0;
            this.insuranceView.SetTabPageEvent += new System.EventHandler( this.SetTabPageEventHandler );
            // 
            // diagnosisView
            // 
            this.diagnosisView.BackColor = System.Drawing.Color.White;
            this.diagnosisView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                              System.Drawing.FontStyle.Regular,
                                                              System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.diagnosisView.Location = new System.Drawing.Point( 0, 0 );
            this.diagnosisView.Model = null;
            this.diagnosisView.Name = "diagnosisView";
            this.diagnosisView.Size = new System.Drawing.Size( 1024, 387 );
            this.diagnosisView.TabIndex = 0;
            this.diagnosisView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // billingView
            // 
            this.billingView.BackColor = System.Drawing.Color.White;
            this.billingView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                            System.Drawing.FontStyle.Regular,
                                                            System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.billingView.Location = new System.Drawing.Point( 0, 0 );
            this.billingView.Model = null;
            this.billingView.Name = "billingView";
            this.billingView.Size = new System.Drawing.Size( 1024, 378 );
            this.billingView.TabIndex = 0;
            this.billingView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // demographicsView
            // 
            this.demographicsView.BackColor = System.Drawing.Color.White;
            this.demographicsView.Location = new System.Drawing.Point( 0, 0 );
            this.demographicsView.Model = null;
            this.demographicsView.Name = "demographicsView";
            this.demographicsView.Size = new System.Drawing.Size( 1024, 378 );
            this.demographicsView.TabIndex = 2;
            this.demographicsView.RefreshTopPanel += new System.EventHandler( this.demographicsView_RefreshTopPanel );
            this.demographicsView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // patientContactsView
            // 
            this.patientContactsView.BackColor = System.Drawing.Color.White;
            this.patientContactsView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                                    System.Drawing.FontStyle.Regular,
                                                                    System.Drawing.GraphicsUnit.Point,
                                                                    ( ( System.Byte )( 0 ) ) );
            this.patientContactsView.Location = new System.Drawing.Point( 0, 0 );
            this.patientContactsView.Model = null;
            this.patientContactsView.Name = "patientContactsView";
            this.patientContactsView.Size = new System.Drawing.Size( 1024, 378 );
            this.patientContactsView.TabIndex = 0;
            this.patientContactsView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // patientEmploymentView
            // 
            this.patientEmploymentView.BackColor = System.Drawing.Color.White;
            this.patientEmploymentView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                                      System.Drawing.FontStyle.Regular,
                                                                      System.Drawing.GraphicsUnit.Point,
                                                                      ( ( System.Byte )( 0 ) ) );
            this.patientEmploymentView.Location = new System.Drawing.Point( 0, 0 );
            this.patientEmploymentView.Model = null;
            this.patientEmploymentView.Model_Account = null;
            this.patientEmploymentView.Name = "patientEmploymentView";
            this.patientEmploymentView.Size = new System.Drawing.Size( 1024, 378 );
            this.patientEmploymentView.TabIndex = 0;
            this.patientEmploymentView.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // registerConfirmationView1
            // 
            this.registerConfirmationView1.BackColor = System.Drawing.Color.White;
            this.registerConfirmationView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registerConfirmationView1.Location = new System.Drawing.Point( 0, 0 );
            this.registerConfirmationView1.Model = null;
            this.registerConfirmationView1.Name = "registerConfirmationView1";
            this.registerConfirmationView1.Size = new System.Drawing.Size( 1006, 526 );
            this.registerConfirmationView1.TabIndex = 0;
            this.registerConfirmationView1.RepeatActivity +=
                new System.EventHandler( this.registerConfirmationView1_RepeatActivity );
            this.registerConfirmationView1.CloseActivity +=
                new System.EventHandler( this.registerConfirmationView1_CloseActivity );
            this.registerConfirmationView1.EditAccount +=
                new System.EventHandler( this.registerConfirmationView1_EditAccount );
            //
            // panelConfirmation
            // 
            this.panelConfirmation.BackColor = System.Drawing.Color.White;
            this.panelConfirmation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelConfirmation.Controls.Add( this.registerConfirmationView1 );
            this.panelConfirmation.Location = new System.Drawing.Point( 8, 56 );
            this.panelConfirmation.Name = "panelConfirmation";
            this.panelConfirmation.Size = new System.Drawing.Size( 1008, 528 );
            this.panelConfirmation.TabIndex = 4;
            // 
            // tcViewTabPages
            // 
            this.tcViewTabPages.Controls.Add( this.tpDemographics );
            this.tcViewTabPages.Controls.Add( this.tpEmployment );
            this.tcViewTabPages.Controls.Add( this.tpDiagnosis );
            this.tcViewTabPages.Controls.Add( this.tpClinical );
            this.tcViewTabPages.Controls.Add( this.tpInsurance );
            this.tcViewTabPages.Controls.Add( this.tpGuarantor );
            this.tcViewTabPages.Controls.Add( this.tpBilling );
            this.tcViewTabPages.Controls.Add( this.tpCounseling );
            this.tcViewTabPages.Controls.Add( this.tpPayment );
            this.tcViewTabPages.Controls.Add( this.tpContacts );
            this.tcViewTabPages.Controls.Add( this.tpRegulatory );
            this.tcViewTabPages.Controls.Add( this.tpDocuments );
            this.tcViewTabPages.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                               System.Drawing.FontStyle.Regular,
                                                               System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.tcViewTabPages.Location = new System.Drawing.Point( 8, 176 );
            this.tcViewTabPages.Name = "tcViewTabPages";
            this.tcViewTabPages.SelectedIndex = 0;
            this.tcViewTabPages.Size = new System.Drawing.Size( 1008, 407 );
            this.tcViewTabPages.TabIndex = 2;
            this.tcViewTabPages.SelectedIndexChanged += new System.EventHandler( this.tcViewTabPages_SelectedIndexChanged );
            // 
            // tpDemographics
            // 
            this.tpDemographics.Controls.Add( this.demographicsView );
            this.tpDemographics.Location = new System.Drawing.Point( 4, 22 );
            this.tpDemographics.Name = "tpDemographics";
            this.tpDemographics.Size = new System.Drawing.Size( 1000, 381 );
            this.tpDemographics.TabIndex = 10;
            this.tpDemographics.Text = "Demographics";
            // 
            // tpEmployment
            // 
            this.tpEmployment.Controls.Add( this.patientEmploymentView );
            this.tpEmployment.Location = new System.Drawing.Point( 4, 22 );
            this.tpEmployment.Name = "tpEmployment";
            this.tpEmployment.Size = new System.Drawing.Size( 1000, 381 );
            this.tpEmployment.TabIndex = 11;
            this.tpEmployment.Text = "Employment";
            // 
            // tpDiagnosis
            // 
            this.tpDiagnosis.Controls.Add( this.diagnosisView );
            this.tpDiagnosis.Location = new System.Drawing.Point( 4, 22 );
            this.tpDiagnosis.Name = "tpDiagnosis";
            this.tpDiagnosis.Size = new System.Drawing.Size( 1000, 381 );
            this.tpDiagnosis.TabIndex = 12;
            this.tpDiagnosis.Text = "Diagnosis";
            // 
            // tpClinical
            // 
            this.tpClinical.Controls.Add( this.clinicalView1 );
            this.tpClinical.Location = new System.Drawing.Point( 4, 22 );
            this.tpClinical.Name = "tpClinical";
            this.tpClinical.Size = new System.Drawing.Size( 1000, 381 );
            this.tpClinical.TabIndex = 13;
            this.tpClinical.Text = "Clinical";
            // 
            // clinicalView1
            // 
            this.clinicalView1.BackColor = System.Drawing.Color.White;
            this.clinicalView1.Location = new System.Drawing.Point( 0, 1 );
            this.clinicalView1.Model = null;
            this.clinicalView1.Model_Account = null;
            this.clinicalView1.Name = "clinicalView1";
            this.clinicalView1.Size = new System.Drawing.Size( 1024, 380 );
            this.clinicalView1.TabIndex = 0;
            this.clinicalView1.FocusOutOfPhysicianSelectArea +=
                new System.EventHandler( this.clinicalView1_FocusOutOfPhysicianSelectArea );
            this.clinicalView1.EnableInsuranceTab += new System.EventHandler( this.EnableInsuranceTabHandler );
            // 
            // tpInsurance
            // 
            this.tpInsurance.Controls.Add( this.insuranceView );
            this.tpInsurance.Location = new System.Drawing.Point( 4, 22 );
            this.tpInsurance.Name = "tpInsurance";
            this.tpInsurance.Size = new System.Drawing.Size( 1000, 381 );
            this.tpInsurance.TabIndex = 14;
            this.tpInsurance.Text = "Insurance";
            // 
            // tpGuarantor
            // 
            this.tpGuarantor.Controls.Add( this.guarantorView );
            this.tpGuarantor.BackColor = System.Drawing.Color.White;
            this.tpGuarantor.Location = new System.Drawing.Point( 4, 22 );
            this.tpGuarantor.Name = "tpGuarantor";
            this.tpGuarantor.Size = new System.Drawing.Size( 1000, 381 );
            this.tpGuarantor.TabIndex = 15;
            this.tpGuarantor.Text = "Guarantor";
            // 
            // tpBilling
            // 
            this.tpBilling.Controls.Add( this.billingView );
            this.tpBilling.Location = new System.Drawing.Point( 4, 22 );
            this.tpBilling.Name = "tpBilling";
            this.tpBilling.Size = new System.Drawing.Size( 1000, 381 );
            this.tpBilling.TabIndex = 16;
            this.tpBilling.Text = "Billing";
            // 
            // tpCounseling
            // 
            this.tpCounseling.BackColor = System.Drawing.Color.White;
            this.tpCounseling.Location = new System.Drawing.Point( 4, 22 );
            this.tpCounseling.Name = "tpCounseling";
            this.tpCounseling.Size = new System.Drawing.Size( 1000, 381 );
            this.tpCounseling.TabIndex = 17;
            this.tpCounseling.Text = "Liability";
            // 
            // tpPayment
            // 
            this.tpPayment.BackColor = System.Drawing.Color.White;
            this.tpPayment.Location = new System.Drawing.Point( 4, 22 );
            this.tpPayment.Name = "tpPayment";
            this.tpPayment.Size = new System.Drawing.Size( 1000, 381 );
            this.tpPayment.TabIndex = 18;
            this.tpPayment.Text = "Payment";
            // 
            // tpContacts
            // 
            this.tpContacts.Controls.Add( this.patientContactsView );
            this.tpContacts.Location = new System.Drawing.Point( 4, 22 );
            this.tpContacts.Name = "tpContacts";
            this.tpContacts.Size = new System.Drawing.Size( 1000, 381 );
            this.tpContacts.TabIndex = 19;
            this.tpContacts.Text = "Contacts";
            // 
            // tpRegulatory
            // 
            this.tpRegulatory.Controls.Add( this.pnlRegulatoryView );
            this.tpRegulatory.Location = new System.Drawing.Point( 4, 22 );
            this.tpRegulatory.Name = "tpRegulatory";
            this.tpRegulatory.Size = new System.Drawing.Size( 1000, 381 );
            this.tpRegulatory.TabIndex = 20;
            this.tpRegulatory.Text = "Regulatory";
            // 
            // pnlRegulatoryView
            // 
            this.pnlRegulatoryView.BackColor = System.Drawing.Color.White;
            this.pnlRegulatoryView.Controls.Add( this.regulatoryView );
            this.pnlRegulatoryView.Location = new System.Drawing.Point( 0, 0 );
            this.pnlRegulatoryView.Name = "pnlRegulatoryView";
            this.pnlRegulatoryView.Size = new System.Drawing.Size( 1024, 378 );
            this.pnlRegulatoryView.TabIndex = 0;
            // 
            // regulatoryView
            // 
            this.regulatoryView.BackColor = System.Drawing.Color.White;
            this.regulatoryView.Location = new System.Drawing.Point( 10, 10 );
            this.regulatoryView.Model = null;
            this.regulatoryView.Name = "regulatoryView";
            this.regulatoryView.Size = new System.Drawing.Size( 1024, 380 );
            this.regulatoryView.TabIndex = 0;
            // 
            // tpDocuments
            // 
            this.tpDocuments.Controls.Add( this.listOfDocumentsView );
            this.tpDocuments.Location = new System.Drawing.Point( 4, 22 );
            this.tpDocuments.Name = "tpDocuments";
            this.tpDocuments.Size = new System.Drawing.Size( 1000, 381 );
            this.tpDocuments.TabIndex = 21;
            this.tpDocuments.Text = "Documents";
            // 
            // panelToDoList
            // 
            this.panelToDoList.Controls.Add( this.lvToDo );
            this.panelToDoList.Controls.Add( this.lblStaticToDoList );
            this.panelToDoList.Controls.Add( this.btnRefreshToDoList );
            this.panelToDoList.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                              System.Drawing.FontStyle.Bold,
                                                              System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.panelToDoList.Location = new System.Drawing.Point( 8, 57 );
            this.panelToDoList.Name = "panelToDoList";
            this.panelToDoList.Size = new System.Drawing.Size( 1005, 108 );
            this.panelToDoList.TabIndex = 1;
            // 
            // lvToDo
            // 
            this.lvToDo.Columns.AddRange( new System.Windows.Forms.ColumnHeader[]
                                             {
                                                 this.chAction,
                                                 this.chCount,
                                                 this.chWorklist
                                             } );
            this.lvToDo.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular,
                                                       System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.lvToDo.FullRowSelect = true;
            this.lvToDo.GridLines = true;
            this.lvToDo.HideSelection = false;
            this.lvToDo.Location = new System.Drawing.Point( 0, 37 );
            this.lvToDo.MultiSelect = false;
            this.lvToDo.Name = "lvToDo";
            this.lvToDo.Size = new System.Drawing.Size( 1003, 66 );
            this.lvToDo.TabIndex = 2;
            this.lvToDo.View = System.Windows.Forms.View.Details;
            this.lvToDo.KeyDown += new System.Windows.Forms.KeyEventHandler( this.ToDoListView_KeyDown );
            this.lvToDo.DoubleClick += new System.EventHandler( this.lvToDo_DoubleClick );
            this.lvToDo.Enter += new System.EventHandler( this.ToDoListView_Enter );
            this.lvToDo.SelectedIndexChanged += new System.EventHandler( this.lvToDo_SelectedIndexChanged );
            // 
            // chAction
            // 
            this.chAction.Text = "Action Item";
            this.chAction.Width = 650;
            // 
            // chCount
            // 
            this.chCount.Text = "Count";
            this.chCount.Width = 75;
            // 
            // chWorklist
            // 
            this.chWorklist.Text = "Worklist";
            this.chWorklist.Width = 250;
            // 
            // lblStaticToDoList
            // 
            this.lblStaticToDoList.Location = new System.Drawing.Point( 0, 11 );
            this.lblStaticToDoList.Name = "lblStaticToDoList";
            this.lblStaticToDoList.Size = new System.Drawing.Size( 70, 23 );
            this.lblStaticToDoList.TabIndex = 1;
            this.lblStaticToDoList.Text = "To Do List";
            this.lblStaticToDoList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRefreshToDoList
            // 
            this.btnRefreshToDoList.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefreshToDoList.Enabled = false;
            this.btnRefreshToDoList.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F,
                                                                   System.Drawing.FontStyle.Regular,
                                                                   System.Drawing.GraphicsUnit.Point,
                                                                   ( ( System.Byte )( 0 ) ) );
            this.btnRefreshToDoList.Location = new System.Drawing.Point( 928, 8 );
            this.btnRefreshToDoList.Name = "btnRefreshToDoList";
            this.btnRefreshToDoList.TabIndex = 1;
            this.btnRefreshToDoList.Text = "Re&fresh";
            this.btnRefreshToDoList.Click += new System.EventHandler( this.btnRefreshToDoList_Click );
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView );
            this.panelPatientContext.Location = new System.Drawing.Point( 8, 29 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size( 1005, 26 );
            this.panelPatientContext.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.btnFinish );
            this.panel1.Controls.Add( this.btnNext );
            this.panel1.Controls.Add( this.btnBack );
            this.panel1.Controls.Add( this.btnCancel );
            this.panel1.Location = new System.Drawing.Point( 0, 585 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1024, 35 );
            this.panel1.TabIndex = 3;
            // 
            // btnFinish
            // 
            this.btnFinish.Anchor =
                ( ( System.Windows.Forms.AnchorStyles )
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnFinish.BackColor = System.Drawing.SystemColors.Control;
            this.btnFinish.Location = new System.Drawing.Point( 937, 6 );
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size( 75, 24 );
            this.btnFinish.TabIndex = 53;
            this.btnFinish.Text = "Fini&sh";
            this.btnFinish.Click += new System.EventHandler( this.btnFinish_Click );
            // 
            // btnNext
            // 
            this.btnNext.Anchor =
                ( ( System.Windows.Forms.AnchorStyles )
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.btnNext.Location = new System.Drawing.Point( 851, 6 );
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size( 75, 24 );
            this.btnNext.TabIndex = 52;
            this.btnNext.Text = "&Next >";
            this.btnNext.Click += new System.EventHandler( this.btnNext_Click );
            // 
            // btnBack
            // 
            this.btnBack.Anchor =
                ( ( System.Windows.Forms.AnchorStyles )
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnBack.BackColor = System.Drawing.SystemColors.Control;
            this.btnBack.Location = new System.Drawing.Point( 771, 6 );
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size( 75, 24 );
            this.btnBack.TabIndex = 51;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += new System.EventHandler( this.btnBack_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor =
                ( ( System.Windows.Forms.AnchorStyles )
                 ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point( 686, 6 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 24 );
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // panelUserContext
            // 
            this.panelUserContext.BackColor = System.Drawing.SystemColors.Control;
            this.panelUserContext.Controls.Add( this.userContextView );
            this.panelUserContext.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserContext.Location = new System.Drawing.Point( 0, 0 );
            this.panelUserContext.Name = "panelUserContext";
            this.panelUserContext.Size = new System.Drawing.Size( 1024, 22 );
            this.panelUserContext.TabIndex = 0;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 12, 198 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 1000, 380 );
            this.progressPanel1.TabIndex = 3;
            // 
            // AccountView
            // 
            this.Load += new EventHandler( AccountView_Load );
            this.Leave += new EventHandler( AccountView_Leave );
            this.BackColor = System.Drawing.Color.FromArgb( ( ( System.Byte )( 209 ) ), ( ( System.Byte )( 228 ) ),
                                                           ( ( System.Byte )( 243 ) ) );
            this.Controls.Add( this.panelUserContext );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.panelPatientContext );
            this.Controls.Add( this.panelToDoList );
            this.Controls.Add( this.tcViewTabPages );
            this.Controls.Add( this.panelConfirmation );
            this.Controls.Add( this.progressPanel1 );
            this.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular,
                                                System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.Name = "AccountView";
            this.Size = new System.Drawing.Size( 1024, 620 );
            this.panelConfirmation.ResumeLayout( false );
            this.tcViewTabPages.ResumeLayout( false );
            this.tpDemographics.ResumeLayout( false );
            this.tpEmployment.ResumeLayout( false );
            this.tpDiagnosis.ResumeLayout( false );
            this.tpClinical.ResumeLayout( false );
            this.tpInsurance.ResumeLayout( false );
            this.tpGuarantor.ResumeLayout( false );
            this.tpContacts.ResumeLayout( false );
            this.tpRegulatory.ResumeLayout( false );
            this.pnlRegulatoryView.ResumeLayout( false );
            this.tpDocuments.ResumeLayout( false );
            this.panelToDoList.ResumeLayout( false );
            this.panelPatientContext.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.panelUserContext.ResumeLayout( false );
            this.ResumeLayout( false );
        }

        #endregion

        #endregion

        #region Construction and Finalization

        public static IAccountView GetInstance()
        {
            if ( IsShortRegAccount
                  //||( Model_Account != null ) 
                )
            {
                return ShortAccountView.GetInstance();
            }
            else if (IsQuickRegistered)
            {
                return QuickAccountView.GetInstance();
            }
            else if (IsPAIWalkinRegistered)
            {
                return PAIWalkinAccountView.GetInstance();
            }
            

            //Nereid, need to modify NewBornRegistrationView to create 
            //if ( IsNewbornAccount )
            //{
            //    return NewBornRegistrationView.GetInsance();
            //}

            // This method is only called from Worklist Action objects.
            // Set flag to know this fact so if Cancel button is clicked,
            // we can go back to the WorklistsView.

            if (c_singletonInstance == null)
            {
                lock (c_syncRoot)
                {
                    if (c_singletonInstance == null)
                    {
                        c_singletonInstance = new AccountView();
                    }
                }
            }
            c_singletonInstance.selectedToDoItemOid = -1;

            return c_singletonInstance;
        }

        public IAccountView GetIInstance()
        {
            return null;
        }

        public static IAccountView NewInstance()
        {
            if ( IsShortRegAccount )
            {
                return ShortAccountView.NewInstance();
            }
            if ( IsQuickRegistered )
            {
                return QuickAccountView.NewInstance();
            }
            if (IsPAIWalkinRegistered)
            {
                return PAIWalkinAccountView.NewInstance();
            }
            if ( c_singletonInstance != null )
            {
                c_singletonInstance.Dispose();
            }
            c_singletonInstance = new AccountView {selectedToDoItemOid = -1};
            return c_singletonInstance;
        }

        protected AccountView()
            : this( new MessageDisplayStateManager(), Rules.RuleEngine.GetInstance() )
        {
        }
        public AccountView( IMessageDisplayStateManager messageStateManager, IRuleEngine ruleEngine )
        {
            EnableInsuranceTab = false;
            MedicareOver65Checked = false;
            Over65Check = false;
            ActivatingTab = string.Empty;
            // This call is required by the Windows.Forms Form Designer.

            SuspendLayout();

            diagnosisView = ViewFactory.Instance.CreateView<DiagnosisView>();

            InitializeComponent();

            Message = Name;

            EnableThemesOn( this );
            WasInvokedFromWorklistItem = false;
            selectedToDoItemOid = -1;
            BackColor = Color.FromArgb( 209, 228, 243 );
            patientContactsView.BackColor = Color.White;
            panelUserContext.BackColor = Color.FromArgb( 209, 228, 243 );
            panelConfirmation.Hide();
            btnCancel.Message = "Click cancel activity";
            btnFinish.Message = "Click finish activity";
            btnRefreshToDoList.Message = "Click refresh TODO list";

            AcceptButton = btnNext;
            btnBack.Enabled = btnNext.Enabled = false;
            btnFinish.Enabled = false;
            ActivityEventAggregator.GetInstance().ErrorMessageDisplayed +=
                ErrorMessageDisplayedForRuleEventHandler;

            ResumeLayout();
            MessageStateManager = messageStateManager;
            RuleEngine = ruleEngine;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                ViewFactory.Instance.CreateView<PatientAccessView>().Model = null;
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );

                c_singletonInstance = null;

                if ( components != null )
                {
                    components.Dispose();
                }

                // cancel the background workers here...
                CancelBackgroundWorkers();

                // SR 41094 - January 2008 Release 
                // Disabled all icons/menu options for previously scanned documents

                ViewFactory.Instance.CreateView<PatientAccessView>().DisablePreviousDocumentOptions();
            }
            //This is a temporary fix until the cause for tabcontrol 
            //ObjectDiposedException issue is figured out.
            try
            {
                base.Dispose( disposing );
            }
            catch ( ObjectDisposedException objDispEx )
            {
                c_log.Error( String.Format( "Message - {0}, StackTrace - {1}", objDispEx.Message,
                                          objDispEx.StackTrace ) );
            }
        }

        #endregion

        #region Data Elements

        private Container components = null;

        private LoggingButton btnRefreshToDoList;
        private ClickOnceLoggingButton btnCancel;
        private ClickOnceLoggingButton btnFinish;
        private LoggingButton btnNext;
        private LoggingButton btnBack;

        private readonly Timer i_BenefitsResponsePollTimer = new Timer();

        private BackgroundWorker backgroundWorker;
        private BackgroundWorker toDoListWorker;

        private ColumnHeader chAction;
        private ColumnHeader chCount;
        private ColumnHeader chWorklist;

        private Label lblStaticToDoList;

        private ListView lvToDo;

        private Panel panelToDoList;
        private Panel panelPatientContext;
        private Panel panel1;
        private Panel panelUserContext;
        private Panel pnlRegulatoryView;
        private Panel panelConfirmation;

        private TabControl tcViewTabPages;

        private TabPage tpClinical;
        private TabPage tpContacts;
        private TabPage tpCounseling;
        private TabPage tpDemographics;
        private TabPage tpDiagnosis;
        private TabPage tpEmployment;
        private TabPage tpGuarantor;
        private TabPage tpBilling;
        private TabPage tpInsurance;
        private TabPage tpPayment;
        private TabPage tpRegulatory;
        private TabPage tpDocuments;

        private BillingView billingView;
        private CopyPartyView copyPartyView;
        private DemographicsView demographicsView;
        private DemographicsEmploymentView patientEmploymentView;
        private readonly DiagnosisView diagnosisView;
        private GuarantorView guarantorView;
        private ClinicalView clinicalView1;
        private InsuranceView insuranceView;
        private PatientContextView patientContextView;
        private PatientContactsView patientContactsView;
        private UserContextView userContextView;
        private RegulatoryView regulatoryView;
        private ListOfDocumentsView listOfDocumentsView;
        private InvalidCodeFieldsDialog invalidCodeFieldsDialog;
        private InvalidCodeOptionalFieldsDialog invalidCodeOptionalFieldsDialog;
        private RegisterConfirmationView registerConfirmationView1;
        private FinancialCouncelingService i_FinancialCouncelingService;
        private static AccountView c_singletonInstance;
        private MaintenanceCmdView maintenanceCmdView;
        private ControlView paymentView;
        private ProgressPanel progressPanel1;

        private IRequiredFieldsSummaryView RequiredFieldSummaryView = new RequiredFieldsSummaryView();
        private RequiredFieldsDialog requiredFieldsDialog;
        private ShowDuplicatePreRegAccountsDialog showDuplicatePreRegAccountsDialog;

        private ControlView controlViewForPayment;
        private ControlView controlViewForLiability;

        private ICollection accountProxiesCollection;


        private static readonly object c_syncRoot = new Object();
        private bool wasInvokedFromWorklistItem;
        private bool initialDisplay = true;
        private long selectedToDoItemOid;
        private string lastKeyPressed;
        private bool blnRulesLoaded = false;
        private ArrayList worklistitems = null;
        private readonly ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();

        private static readonly ILog c_log = LogManager.GetLogger( typeof( AccountView ) );

        // Tab index values used by MSP Wizard to invoke desired TabPage
        public static int EMPLOYMENT = ( int )ScreenIndexes.EMPLOYMENT;

        public static int INSURANCE = ( int )ScreenIndexes.INSURANCE;
        private static bool i_IsShortRegAccount;

        

        public enum ScreenIndexes
        {
            DEMOGRAPHICS,
            EMPLOYMENT,
            DIAGNOSIS,
            CLINICAL,
            INSURANCE,
            GUARANTOR,
            BILLING,
            COUNSELING,
            PAYMENT,
            CONTACTS,
            REGULATORY,
            DOCUMENTS,
            INSURED, // Not a tab index - used by MSP Wizard button event
            PAYORDETAILS, // Not a tab index - used by MSP Wizard button event
            SECONDARYINSURED, // Not a tab index - used by MSP Wizard button event
            SECONDARYPAYORDETAILS, // Not a tab index - used by MSP Wizard button event
            PRIMARYINSVERIFICATION, // Not a tab index - used by MSP Wizard button event
            SECONDARYINSVERIFICATION, // Not a tab index - used by MSP Wizard button event
            PRIMARYAUTHORIZATION, // Not a tab index - used by MSP Wizard button event
            SECONDARYAUTHORIZATION, // Not a tab index - used by MSP Wizard button event
            REFERRINGNONSTAFFPHYSICIAN,
            ADMITTINGNONSTAFFPHYSICIAN,
            ATTENDINGNONSTAFFPHYSICIAN,
            OPERATINGNONSTAFFPHYSICIAN,
            PRIMARYCARENONSTAFFPHYSICIAN
        } ;

        #endregion

        #region Constants

        private const int RESPONSE_POLL_INTERVAL = 20000; // 20 seconds

        private const string RIGHTARROW = "RightArrow";
        private const string LEFTARROW = "LeftArrow";

        private const int AGE_SIXTY_FIVE = 65;

        #endregion
    }
}