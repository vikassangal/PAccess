using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Builder;
using PatientAccess.Actions;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.FinancialCounselingViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PhysicianSearchViews;
using PatientAccess.UI.PreRegistrationViews;
using PatientAccess.UI.WorklistViews;
using log4net;
using OnQuickAccountCreationForm = PatientAccess.Rules.OnQuickAccountCreationForm;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    [UsedImplicitly]
    public partial class QuickAccountView : LoggingControlView, IAccountView
    {
        #region Events

        public event EventHandler OnRepeatActivity;
        public event EventHandler OnEditAccount;
        public event EventHandler OnCloseActivity;
     
        #endregion

        #region Event Handlers


        public void StartBenefitsResponsePollTimer()
        {
            
        }
        private void AccountView_Leave( object sender, EventArgs e )
        {
            // cancel both background workers on leaving the view
            CancelBackgroundWorkers();
        }

        private void AccountView_Load( object sender, EventArgs e )
        {
            tcViewTabPages.SelectedIndex = -1;
        }

        private void SetRelationshipTypeForEmergencyContact()
        {
            if (Model_Account.EmergencyContact1 != null)
            {
                foreach (Relationship relationship in Model_Account.EmergencyContact1.Relationships)
                {
                    Model_Account.EmergencyContact1.RelationshipType = relationship.Type;
                }
            }
            if (Model_Account.EmergencyContact2 != null)
            {
                foreach (Relationship relationship in Model_Account.EmergencyContact2.Relationships)
                {
                    Model_Account.EmergencyContact2.RelationshipType = relationship.Type;
                }
            }
        }

        private void SetRelationshipTypeForGuarantor()
        {
            if (Model_Account.Guarantor != null)
            {
                foreach (Relationship relationship in Model_Account.Guarantor.Relationships)
                {
                    Model_Account.GuarantorIs(Model_Account.Guarantor, relationship.Type);
                }
            }
        }
        private void quickAccountCreationView_RefreshTopPanel( object sender, EventArgs e )
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
                var result = MessageBox.Show(
                    UIErrorMessages.INCOMPLETE_ACTIVITY_MSG,
                    UIErrorMessages.ACTIVITY_DIALOG_TITLE, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation );

                if ( result == DialogResult.Yes )
                {
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
                        var aggregator =
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
                    Dispose();
                }
                else
                {
                    btnCancel.Enabled = true;
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

        private void btnFinish_Click( object sender, EventArgs e )
        {
            Model_Account.FinancialClass = new FinancialClass( 296L, PersistentModel.NEW_VERSION, "MED (MSE) SCREEN EXM", MED_MSE_SCREEN_EXM );
            SetRelationshipTypeForEmergencyContact();
            SetRelationshipTypeForGuarantor();
            DoNotProceedWithFinish = false;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
            
            if ( DoNotProceedWithFinish )
            {
                return;
            }
        
            if ( DoNotProceedWithFinish )
            {
                ReEnableFinishButtonAndFusIcon();
                return;
            }
            //TODO-AC end todo

            Cursor = Cursors.WaitCursor;

            SetActivatingTab( string.Empty );
            var isPhysicianValid = PhysiciansValidated();

            if ( isPhysicianValid != true )
            {
                ReEnableFinishButtonAndFusIcon();
                return;
            }

            if ( !RuleEngine.EvaluateAllRules( Model_Account ) )
            {
                var inValidCodesSummary = RuleEngine.GetInvalidCodeFieldSummary();

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

                var summary = RuleEngine.GetRemainingErrorsSummary();

                if ( summary != String.Empty )
                {
                    requiredFieldsDialog = new RequiredFieldsDialog
                                               {
                                                   Title = "Warning for Remaining Errors",
                                                   HeaderText = "The fields listed have values that are creating errors. Visit each field\r\n" + "listed and correct the value before completing this activity.",
                                                   ErrorText = summary
                                               };
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

                var collection = RuleEngine.GetCompositeItemsCollection();

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

                    quickAccountCreationView.RequiredFieldsSummaryPresenter = RequiredFieldsSummaryPresenter;

                    RequiredFieldsSummaryPresenter.ShowViewAsDialog( this );
                    RuleEngine.ClearActions();
                    runRulesForTab();
                    ReEnableFinishButtonAndFusIcon();

                    return;
                }
            }

            // BUG 1512 Fix - Verify if SpanCodes 70 and 71 are still valid for
            // the current Patient Type, in case the Patient Type was changed,
            // and 'Finish' clicked, without going to the Billing View.

            var patient = Model_Account.Patient;
            patient.SelectedAccount = Model_Account;

            if ( Model_Account != null &&
                 !Model_Account.Activity.GetType().Equals( typeof( EditAccountActivity ) ) &&
                 !Model_Account.Activity.GetType().Equals( typeof( MaintenanceActivity ) ) )
            {
                patient.ClearPriorSystemGeneratedOccurrenceSpans();

                var spanCode70 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
                var spanCode71 = scBroker.SpanCodeWith( User.GetCurrent().Facility.Oid, SpanCode.PRIOR_STAY_DATES );

                patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );
            }

            // BUG 1512 Fix - End

            //Raise Activitycomplete event
            if ( !checkForError()  )
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
             
                SaveAccount();
            }
            else
            {
                ReEnableFinishButtonAndFusIcon();
                return;
            }

            RuleEngine.UnloadHandlers();

            // Close Account Supplemental Information View if open for Online PreRegistration Account creation
            ViewFactory.Instance.CreateView<PatientAccessView>().CloseAccountSupplementalInformationView();

            Cursor = Cursors.Default;
        }
       
        private void requiredFieldSummary_TabSelectedEvent( object sender, EventArgs e )
        {
            var index = ( int )( ( LooseArgs )e ).Context;

            // only process those on this view (tab indexes 0 - 11)

            if ( index < 1 )
            {
                tcViewTabPages.SelectedIndex = index;
                tcViewTabPages.TabPages[index].Select();
                processTab();
            }
        }

        private void lvToDo_SelectedIndexChanged( object sender, EventArgs e )
        {
            var selectedItemDesc = string.Empty;

            var listView = sender as ListView;
            if ( initialDisplay )
            {
                initialDisplay = false;
            }
            if ( listView != null && listView.SelectedItems.Count > 0 && listView.SelectedItems[0].Tag != null )
            {
                if ( listView.SelectedItems[0].Tag is CompositeAction )
                {
                    var compositeAction = ( CompositeAction )listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = compositeAction.Oid;
                    selectedItemDesc = compositeAction.Description;
                }
                else if ( listView.SelectedItems[0].Tag is LeafAction )
                {
                    var leafAction = ( LeafAction )listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = leafAction.Oid;
                    selectedItemDesc = leafAction.Description;
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

        /// <summary>
        /// On tab focus to the ListView, set first item to the selected state.
        /// (Windows Forms doesn't do this the same way a mouse click does.)
        /// </summary>
        private void ToDoListView_Enter( object sender, EventArgs e )
        {
            var listView = sender as ListView;
            if ( initialDisplay )
            {
                // This handler is invoked when form is painted.  Requirements
                // dictate that no action item is selected on first display
                return;
            }

            if ( listView == null ) return;

            if ( listView.Items.Count == 0 )
            {
                return;
            }

            if (listView.SelectedItems.Count != 0)
            {
                listView.Items[0].Selected = true;
            }

            if (listView.SelectedItems.Count > 0 && listView.SelectedItems[0].Tag != null)
            {
                if (listView.SelectedItems[0].Tag is CompositeAction)
                {
                    var compositeAction = (CompositeAction) listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = compositeAction.Oid;
                }
                else if (listView.SelectedItems[0].Tag is LeafAction)
                {
                    var leafAction = (LeafAction) listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = leafAction.Oid;
                }
            }
        }

        /// <summary>
        /// If an item in the ToDo list is selected by the keyboard, set it to the selected state.
        /// (Windows Forms doesn't do this the same way a mouse click does.)
        /// </summary>
        private void ToDoListView_KeyDown( object sender, KeyEventArgs e )
        {
            var listView = sender as ListView;
            if ( listView == null ) return;

            if ( listView.SelectedItems.Count > 0 && listView.SelectedItems[0].Tag != null )
            {
                if ( listView.SelectedItems[0].Tag is CompositeAction )
                {
                    var compositeAction = ( CompositeAction )listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = compositeAction.Oid;
                }
                else if ( listView.SelectedItems[0].Tag is LeafAction )
                {
                    var leafAction = ( LeafAction )listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = leafAction.Oid;
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
                var selectedAccount = ( ( LooseArgs )e ).Context as IAccount;

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

        private void ErrorMessageDisplayedForRuleEventHandler( object sender, EventArgs e )
        {
            var args = e as LooseArgs;
            if ( args != null && args.Context != null )
            {
                var rule = args.Context.ToString();
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
        
        public void ActivateEmploymentTab()
        {
            // Dummy method to implement IAccountView interface.
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

            var searchEventAggregator = SearchEventAggregator.GetInstance();

            searchEventAggregator.RaiseActivatePreregisteredAccountEvent( this, new LooseArgs( Model_Account ) );
        }

        public void ActivateDemographicsTab()
        {
            // tcViewTabPages_SelectedIndexChanged() fires calling UpdateView()
            if ( Model_Account.Activity == null )
            {
                Model_Account.Activity = new QuickAccountMaintenanceActivity();
            }

            tcViewTabPages.SelectedIndex = ( int )QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;

            var populationAggregator = PatientAccessViewPopulationAggregator.GetInstance();

            populationAggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );

            UpdateView();
        }

        public void ActivateDiagnosisTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateInsuranceTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateGuarantorTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateCounselingTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivatePaymentTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateContactsTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateClinicalTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateRegulatoryTab()
        {
            // Dummy method to implement IAccountView interface.
        }

        public void ActivateDocumentsTab()
        {
            // Dummy method to implement IAccountView interface.
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

        public bool WasInvokedFromWorklistItem { private get; set; }

        public string ActiveContext
        {
            set { userContextView.Description = value; }
        }
        public BackgroundWorker SavingAccountBackgroundWorker
        {
            get { return backgroundWorker; }
        }

        private IMessageDisplayStateManager MessageStateManager { get; set; }
        private IRuleEngine RuleEngine { get; set; }
        private string ActivatingTab { get; set; }
        private bool DoNotProceedWithFinish { get; set; }
        public bool Over65Check { private get; set; }
        public bool MedicareOver65Checked { get; set; }
        public bool EnableInsuranceTab { private get; set; }
        public IRequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter { get; private set; }
        
        #endregion

        #region Private Properties
        #endregion

        #region Private Methods

        private void SetActiveTab()
        {
            int index;

            SuspendLayout();

            switch ( ActivatingTab )
            {

                case "QuickAccountCreation":
                    {
                        index = ( int )QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;
                        break;
                    }
                case "Payor":
                    {
                        index = (int)QuickAccountCreationScreenIndexes.PAYORDETAILS;
                        break;
                    }
                case "Insured":
                    {
                        index = (int)QuickAccountCreationScreenIndexes.INSURED;
                        break;
                    }
                
                case "PrimaryInsuranceVerification":
                    {
                        index = (int)QuickAccountCreationScreenIndexes.PRIMARYINSVERIFICATION;
                        break;
                    }
               
                case "PrimaryAuthorization":
                    {
                        index = (int)QuickAccountCreationScreenIndexes.PRIMARYAUTHORIZATION;
                        break;
                    }
               
                default:
                    index = ( int )QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;
                    break;
            }

            SetTabPageEventHandler( this, new LooseArgs( index ) );
        }
    
        /// <summary>
        /// Fired from InsuranceView when the MSP Wizard does a button click.
        /// Wizard raises event on parent form, FinancialClassesView which raises
        /// this event to display the desired tab page.
        /// </summary>
        private void SetTabPageEventHandler( object sender, EventArgs e )
        {
            var args = e as LooseArgs;
            if ( args == null ) return;

            var selectedScreenIndex = (int)args.Context;
      

            switch ( (QuickAccountCreationScreenIndexes)selectedScreenIndex )
            {
                case QuickAccountCreationScreenIndexes.INSURED:
                    DisplayInsuredDialogPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

          

                case QuickAccountCreationScreenIndexes.PAYORDETAILS:
                    DisplayPayorDetailsDialogPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

        

                case QuickAccountCreationScreenIndexes.PRIMARYINSVERIFICATION:
                    DisplayInsuranceVerificationPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

           

                case QuickAccountCreationScreenIndexes.PRIMARYAUTHORIZATION:
                    DisplayInsuranceAuthorizationPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;

          
                default:
                    tcViewTabPages.SelectedIndex = selectedScreenIndex;
                    break;
            }
        }

        /// <summary>
        /// The MSP Wizard dialog has a button to display the InsuranceDetails dialog's
        /// Insured tabPage.  This is called from SetTabPageEventHandler.
        /// </summary>
        private void DisplayInsuranceVerificationPage( CoverageOrder coverageOrder )
        {
            tcViewTabPages.SelectedIndex = (int)QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;

            var insuranceDetails = new InsuranceDetails
            {
                insuranceDetailsView =
                {
                    Model_Coverage = ( coverageOrder.Oid == CoverageOrder.PRIMARY_OID )
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
            tcViewTabPages.SelectedIndex = (int)QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;

            var insuranceDetails = new InsuranceDetails
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
            tcViewTabPages.SelectedIndex = (int)QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;

            var insuranceDetails = new InsuranceDetails
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
            tcViewTabPages.SelectedIndex = (int)QuickAccountCreationScreenIndexes.QUICKACCOUNTCREATION;

            var insuranceDetails = new InsuranceDetails
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
            var anAccount = Model_Account;
            var currentActivity = Model_Account.Activity;
            anAccount.SetDefaultInsurancePlan();
            currentActivity.AppUser = User.GetCurrent();
            var coverageDefaults = new CoverageDefaults();
            coverageDefaults.SetCoverageDefaultsForActivity( anAccount );

            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
         
            if ( accountBroker != null )
            {
                var results = accountBroker.Save( anAccount, currentActivity );
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
            var result = true;
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
            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeSaveAccount();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

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

        private bool checkForError()
        {
            var rcErrors = RuleEngine.AccountHasFailedError();
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
                            const int loading = 1;
                            m.Result = ( IntPtr )loading;
                            return;
                        }

                        //this needs to be initialized to avoid compiler warning 
                        //#0649 "Field 'field' is never assigned to, and will always have its default value 'value'" as an error
                        // for the struct NHDR
                        // ReSharper disable RedundantAssignment
                        var nmh = new NMHDR {hwndFrom = IntPtr.Zero, idFrom = 0, code = 0};
                        // ReSharper restore RedundantAssignment

                        nmh = ( NMHDR )m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            if ( !checkForError() )
                            {
                                Validate();
                            } 
                       
                            const int irc = 0;

                            m.Result = ( IntPtr )irc;
                        }

                        break;
                    }
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
                var compositeAction = lvToDo.SelectedItems[0].Tag as CompositeAction;

                if ( compositeAction == null )
                {
                    var action = lvToDo.SelectedItems[0].Tag as IAction;
                    if ( action != null )
                    {
                        Cursor = Cursors.WaitCursor;
                        action.Execute();
                        SetActivatingTab( action.Context.ToString() );
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    Cursor = Cursors.WaitCursor;
                    compositeAction.Execute();
                    SetActivatingTab( compositeAction.Context.ToString() );
                    Cursor = Cursors.Default;
                }

                SetActiveTab();
            }
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
                if ( toDoListWorker == null || !toDoListWorker.IsBusy )
                {
                    toDoListWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

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
            var kindOfVisitCode = string.Empty;
            var financialClassCode = string.Empty;

            var ruleBrokerProxy = new RuleBrokerProxy();
            var worklistSettingsBroker = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();

            if ( Model_Account.KindOfVisit != null )
            {
                kindOfVisitCode = Model_Account.KindOfVisit.Code;
            }
            if ( Model_Account.FinancialClass != null )
            {
                financialClassCode = Model_Account.FinancialClass.Code;
            }

            Hashtable actionWorklistMapping = ruleBrokerProxy.ActionWorklistMapping( kindOfVisitCode, financialClassCode );
            var worklists = worklistSettingsBroker.GetAllWorkLists();
            worklistitems = new ArrayList();

            // run the rules!
            if ( Model_Account != null )
            {
                var worklistActionItems = RuleEngine.GetWorklistActionItems( Model_Account );
                worklistActionItems.Sort();

                // break out if the bg worker is cancelling

                if ( toDoListWorker != null
                    && toDoListWorker.CancellationPending )
                {
                    return;
                }

                if ( worklistActionItems.Count > 0 )
                {
                    foreach ( LeafAction la in worklistActionItems )
                    {
                        // break out if the bg worker is cancelling

                        if ( toDoListWorker != null
                            && toDoListWorker.CancellationPending )
                        {
                            return;
                        }

                        var arrayList = ( ArrayList )actionWorklistMapping[la.Oid];

                        string worklistName;
                        if ( arrayList != null )
                        {
                            worklistName = ( string )arrayList[1];
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
                            var compositeAction = ( CompositeAction )la;
                            var listViewItem = new ListViewItem { Tag = compositeAction, Text = compositeAction.Description };
                            listViewItem.SubItems.Add( compositeAction.NumberOfAllLeafActions().ToString() );
                            listViewItem.SubItems.Add( worklistName );
                            worklistitems.Add( listViewItem );
                            
                        }
                        else
                        {
                            // only 'concrete' actions that are not composited get added to the list;
                            // generic actions not composited do not

                            if ( la.GetType() != typeof( GenericAction ) )
                            {
                                var listViewItem = new ListViewItem { Tag = la, Text = la.Description };
                                listViewItem.SubItems.Add( "1" );
                                listViewItem.SubItems.Add( worklistName );
                                worklistitems.Add( listViewItem );
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
                var comboBox = c as ComboBox;
                if ( comboBox != null )
                {
                    comboBox.BeginUpdate();
                    c_log.DebugFormat( "Setting BeginUpdate for {0}", comboBox.Name );
                }
                BeginUpdateForAllControlsIn( c.Controls );
            }
        }

        private void EndUpdateForAllControlsIn( ControlCollection aControlsCollection )
        {
            foreach ( Control c in aControlsCollection )
            {
                var comboBox = c as ComboBox;
                if ( comboBox != null )
                {
                    comboBox.EndUpdate();
                    c_log.DebugFormat( "Setting EndUpdate for {0}", comboBox.Name );
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


                if ( tcViewTabPages.SelectedTab == tpQuickAccountCreation )
                {

                    quickAccountCreationView.Model = Model_Account;
                    quickAccountCreationView.UpdateView();
                    
                    quickAccountCreationView.Focus();
                    btnNext.Enabled = false;
                    btnBack.Enabled = false;
                    btnFinish.Enabled = true;

                }

            }
            finally
            {
                EndUpdateForAllControlsIn( Controls );
                Cursor = Cursors.Default;
            }
        }
       
        /// <summary>
        /// UpdateView2 method called from UpdateView.
        /// </summary>
        private void UpdateView2()
        {
            if ( Model_Account != null )
            {
                

                // if this account has a bed, then force retrieval of accomodation codes for
                // quicker loading of the Diagnosis view

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
                    var foundItem = false;
                    var lvic = lvToDo.Items;

                    // Find the item the was the last selected item
                    for ( var index = 0; index < lvic.Count; index++ )
                    {
                        var lvi = lvic[index];
                        if ( lvi.Tag == null )
                        {
                            continue;
                        }
                        
                        if ( lvi.Tag is CompositeAction )
                        {
                            var compositeAction = lvi.Tag as CompositeAction;

                            if ( compositeAction.Oid == selectedToDoItemOid )
                            {
                                lvToDo.Items[index].Selected = true;
                                foundItem = true;
                                break;
                            }
                        }
                        else if ( lvi.Tag is LeafAction )
                        {
                            var leafAction = lvi.Tag as LeafAction;

                            if ( leafAction.Oid == selectedToDoItemOid )
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
            //The below code to avid users clicking on the button before the account view is loaded completely
            btnRefreshToDoList.Enabled = true;
        }

        private void runRulesForTab()
        {
            if ( tcViewTabPages.SelectedTab == tpQuickAccountCreation )
            {
                RuleEngine.EvaluateRule( typeof( OnQuickAccountCreationForm ), Model_Account );
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
                                                     PersistentModel.NEW_OID.ToString(), PersistentModel.NEW_OID.ToString(),
                                                     PersistentModel.NEW_OID.ToString() );
        }

        private string GetAdmittingPhysicianId()
        {
            var result = string.Empty;
            if ( Model_Account != null && Model_Account.AdmittingPhysician != null )
            {
                result = Model_Account.AdmittingPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        private string GetReferringPhysicianId()
        {
            var result = string.Empty;

            if ( Model_Account != null && Model_Account.ReferringPhysician != null )
            {
                result = Model_Account.ReferringPhysician.PhysicianNumber.ToString();
            }
            return result;
        }

        #endregion

        #region Construction and Finalization

        public static QuickAccountView GetInstance()
        {
            // This method is only called from Worklist Action objects.
            // Set flag to know this fact so if Cancel button is clicked,
            // we can go back to the WorklistsView.

            if ( c_singletonInstance == null )
            {
                lock ( c_syncRoot )
                {
                    if ( c_singletonInstance == null )
                    {
                        c_singletonInstance = new QuickAccountView();
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

        public static QuickAccountView NewInstance()
        {
            if ( c_singletonInstance != null )
            {
                c_singletonInstance.Dispose();
            }
            c_singletonInstance = new QuickAccountView { selectedToDoItemOid = -1 };
            return c_singletonInstance;
        }

        private QuickAccountView() : this( new MessageDisplayStateManager(), Rules.RuleEngine.GetInstance() )
        {
        }

        private QuickAccountView( IMessageDisplayStateManager messageStateManager, IRuleEngine ruleEngine )
        {
            EnableInsuranceTab = false;
            MedicareOver65Checked = false;
            Over65Check = false;
            ActivatingTab = string.Empty;
            // This call is required by the Windows.Forms Form Designer.

            SuspendLayout();

           

            InitializeComponent();

            Message = Name;

            EnableThemesOn( this );
            WasInvokedFromWorklistItem = false;
            selectedToDoItemOid = -1;
            BackColor = Color.FromArgb( 209, 228, 243 );
            panelUserContext.BackColor = Color.FromArgb( 209, 228, 243 );
            panelConfirmation.Hide();
            btnCancel.Message = "Click cancel activity";
            btnFinish.Message = "Click finish activity";
            btnRefreshToDoList.Message = "Click refresh TODO list";

          
            btnFinish.Enabled = false;
            ActivityEventAggregator.GetInstance().ErrorMessageDisplayed +=
                ErrorMessageDisplayedForRuleEventHandler;

            ResumeLayout();
            MessageStateManager = messageStateManager;
            RuleEngine = ruleEngine;
        }

       
        #endregion

        #region Data Elements

       
        private readonly Timer i_BenefitsResponsePollTimer = new Timer();
        private BackgroundWorker backgroundWorker;
        private BackgroundWorker toDoListWorker;
        private static QuickAccountView c_singletonInstance;
        private ICollection accountProxiesCollection;
        private static readonly object c_syncRoot = new Object();
        private bool initialDisplay = true;
        private long selectedToDoItemOid;
        private string lastKeyPressed;
        private bool blnRulesLoaded;
        private ArrayList worklistitems;
       
        private readonly ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
        private static readonly ILog c_log = LogManager.GetLogger( typeof( QuickAccountView ) );
        //private InterfacilityTransfer.InterfacilityPannel interfacilityPannel;
        public enum QuickAccountCreationScreenIndexes
        {
            QUICKACCOUNTCREATION,
            INSURED,
            PAYORDETAILS, // Not a tab index - used by MSP Wizard button event
            PRIMARYINSVERIFICATION, // Not a tab index - used by MSP Wizard button event
            PRIMARYAUTHORIZATION, // Not a tab index - used by MSP Wizard button event
            REFERRINGNONSTAFFPHYSICIAN,
            ADMITTINGNONSTAFFPHYSICIAN,
         
        } ;

        #endregion

        #region Constants

        private const string RIGHTARROW = "RightArrow";
        private const string LEFTARROW = "LeftArrow";
        private const string MED_MSE_SCREEN_EXM = "37";
    
        #endregion

        #region IAccountView Members


        public bool IsMedicareAdvisedForPatient()
        {
            throw new NotImplementedException();
        }

        public void DisplayLocationRequiredFieldSummary()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
