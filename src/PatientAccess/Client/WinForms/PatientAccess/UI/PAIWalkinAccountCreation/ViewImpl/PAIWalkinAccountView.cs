using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Builder;
using PatientAccess.Actions;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.FinancialCounselingViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PAIWalkinAccountCreation.Presenters;
using PatientAccess.UI.PAIWalkinAccountCreation.Views;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.WorklistViews;
using log4net;
using OnPAIWalkinAccountCreationForm = PatientAccess.Rules.OnPAIWalkinAccountCreationForm;

namespace PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl
{
    [UsedImplicitly]
    public partial class PAIWalkinAccountView : LoggingControlView, IPAIWalkinAccountView, IAccountView
    {
        #region Events

        public event EventHandler OnRepeatActivity;
        public event EventHandler OnEditAccount;
        public event EventHandler OnCloseActivity;

        #endregion

        #region Event Handlers

        private void AccountView_Leave( object sender, EventArgs e )
        {
            // cancel both background workers on leaving the view
            CancelBackgroundWorkers();
        }

        private void AccountView_Load( object sender, EventArgs e )
        {
            tcViewTabPages.SelectedIndex = -1;
        }

        private void btnActivateStandardRegistration_Click( object sender, EventArgs e )
        {
            shortActivation = false;
            SetActiveButtons( false );
            paiWalkinAccountViewPresenter.HandleFinish();
        }

        private void btnActivateShortRegistration_Click( object sender, EventArgs e )
        {
            shortActivation = true;
            SetActiveButtons( false );
            paiWalkinAccountViewPresenter.HandleFinish();
        }


        private void paiWalkinAccountCreationView_RefreshTopPanel( object sender, EventArgs e )
        {
            patientContextView.GenderLabelText = String.Empty;
            patientContextView.PatientNameText = String.Empty;

            if ( ( ( (LooseArgs)e ).Context ) != null )
            {
                if ( ( (Patient)
                     ( ( (LooseArgs)e ).Context ) ).Sex.Description != String.Empty &&
                    ( (Patient)
                     ( ( (LooseArgs)e ).Context ) ).Sex.Description != String.Empty )
                {
                    patientContextView.GenderLabelText = ( (Patient)
                                                               ( ( (LooseArgs)e ).Context ) ).Sex.Description;
                }

                patientContextView.PatientNameText = ( (Patient)
                                                           ( ( (LooseArgs)e ).Context ) ).Name.AsFormattedName();

                patientContextView.DateOfBirthText = ( (Patient)
                                                           ( ( (LooseArgs)e ).Context ) ).DateOfBirth.Date.ToString
                    ( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );

                patientContextView.SocialSecurityNumber = ( (Patient)
                                                                ( ( (LooseArgs)e ).Context ) ).SocialSecurityNumber.
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

        private void requiredFieldSummary_TabSelectedEvent( object sender, EventArgs e )
        {
            var index = (int)( (LooseArgs)e ).Context;

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
                    var compositeAction = (CompositeAction)listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = compositeAction.Oid;
                    selectedItemDesc = compositeAction.Description;
                }
                else if ( listView.SelectedItems[0].Tag is LeafAction )
                {
                    var leafAction = (LeafAction)listView.SelectedItems[0].Tag;
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

            if ( listView.SelectedItems.Count != 0 )
            {
                listView.Items[0].Selected = true;
            }

            if ( listView.SelectedItems.Count > 0 && listView.SelectedItems[0].Tag != null )
            {
                if ( listView.SelectedItems[0].Tag is CompositeAction )
                {
                    var compositeAction = (CompositeAction)listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = compositeAction.Oid;
                }
                else if ( listView.SelectedItems[0].Tag is LeafAction )
                {
                    var leafAction = (LeafAction)listView.SelectedItems[0].Tag;
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
                    var compositeAction = (CompositeAction)listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = compositeAction.Oid;
                }
                else if ( listView.SelectedItems[0].Tag is LeafAction )
                {
                    var leafAction = (LeafAction)listView.SelectedItems[0].Tag;
                    selectedToDoItemOid = leafAction.Oid;
                }
            }
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

        public override void UpdateView()
        {
            paiWalkinAccountViewPresenter = new PAIWalkinAccountViewPresenter( this );
            FinancialCouncelingService.PriorAccountsRetrieved = false;
            UpdateViewInternal();
            ViewFactory.Instance.CreateView<PatientAccessView>().Model = Model_Account;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions( Model_Account );
        }

        public override void UpdateModel()
        {
        }

        private void DoSaveAccount( object sender, DoWorkEventArgs e )
        {
            if ( !SaveSuccess )
            {
                Model_Account.Facility = User.GetCurrent().Facility;
                var anAccount = Model_Account;
                var currentActivity = Model_Account.Activity;
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
                    SaveSuccess = true;
                }

                // poll CancellationPending and set e.Cancel to true and return 
                if ( backgroundWorker.CancellationPending )
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void HandleActivateStandardRegistration()
        {
            Cursor = Cursors.WaitCursor;

            SelectedAccountProxy = paiWalkinAccountViewPresenter.GetLatestAccountProxyForSelectedAccount();

            if ( SelectedAccountProxy == null )
            {
                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED_MOMENTARILY, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                Cursor = Cursors.Default;
                SetActiveButtons( true );
                return;
            }

            // ensure that the account status has not changed since the user retrieved the account
            // 1. if account is no longer PreReg
            // 2. if account is locked

            if ( SelectedAccountProxy.KindOfVisit.Code != VisitType.PREREG_PATIENT )
            {
                MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_PREREG, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );

                Cursor = Cursors.Default;
                return;
            }

            if ( AccountLockStatus.IsAccountLocked( SelectedAccountProxy, User.GetCurrent() ) )
            {
                Cursor = Cursors.Default;

                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED_MOMENTARILY, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
                SetActiveButtons( true );
                return;
            }

            else
            {
                //***************************************************************************************
                //SR1471, When the button is clicked, always go to RegistrationActivity.
                SelectedAccountProxy.Activity = new MaintenanceActivity { AssociatedActivityType = typeof( RegistrationActivity ) };
                // Pro-actively lock the account proxy
                if ( !SelectedAccountProxy.Activity.ReadOnlyAccount() )
                {
                    bool blnLocked = AccountActivityService.PlaceLockOn( SelectedAccountProxy, String.Empty );

                    if ( !blnLocked )
                    {
                        Cursor = Cursors.Default;
                        SetActiveButtons( true );
                        return;
                    }
                }
                //***************************************************************************************

                IAccount account = SelectedAccountProxy;
                account.IsShortRegistered = false;
                //account.IsShortRegisteredNonDayCareAccount(); 
                Cursor = Cursors.Default;

                //SR1471, When the button is clicked, always go to RegistrationActivity.

                AccountView.IsShortRegAccount = false;
                Activity currentActivity = null;
                if ( account.IsNewBorn )
                {
                    account.Activity = new AdmitNewbornActivity { AssociatedActivityType = typeof( ActivatePreRegistrationActivity ) };
                    currentActivity = new AdmitNewbornActivity();
                }
                else
                {
                    account.Activity = new RegistrationActivity { AssociatedActivityType = typeof( ActivatePreRegistrationActivity ) };
                    currentActivity = new RegistrationActivity();
                }

                if ( ParentForm != null )
                {
                    ( (PatientAccessView)ParentForm ).LoadRegistrationView( currentActivity );
                }

                // Verify the account has not already been canceled
                if ( account.IsCanceled )
                {
                    MessageBox.Show( UIErrorMessages.ACCOUNT_CANCELED_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1 );


                }
                else
                {
                    // raise the ActivatePreregisteredAccountEvent
                    SearchEventAggregator aggregator = SearchEventAggregator.GetInstance();

                    aggregator.RaiseActivatePreregisteredAccountEvent( this, new LooseArgs( SelectedAccountProxy ) );
                    Dispose();
                }
            }
            SetActiveButtons( true );
            Cursor = Cursors.Default;
        }

        void IAccountView.ActivateAccount()
        {
            ActivateAccount();
        }

        public void SetActiveButtons( bool enable )
        {
            btnActivateStandardRegistration.Enabled = enable;
            btnActivateShortRegistration.Enabled = enable;
            btnCancel.Enabled = enable;
        }

        private void HandleActivateShortRegistration()
        {
            Cursor = Cursors.WaitCursor;

            SelectedAccountProxy = paiWalkinAccountViewPresenter.GetLatestAccountProxyForSelectedAccount();

            if ( SelectedAccountProxy == null )
            {
                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED_MOMENTARILY, "Warning",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning,
                   MessageBoxDefaultButton.Button1);
                Cursor = Cursors.Default;
                SetActiveButtons( true );
                return;
            }

            // ensure that the account status has not changed since the user retrieved the account
            // 1. if account is no longer PreReg
            // 2. if account is locked


            if ( SelectedAccountProxy.KindOfVisit.Code != VisitType.PREREG_PATIENT )
            {
                MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_PREREG, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );

                Cursor = Cursors.Default;
                return;
            }

            if ( AccountLockStatus.IsAccountLocked( SelectedAccountProxy, User.GetCurrent() ) )
            {
                Cursor = Cursors.Default;

                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED_MOMENTARILY, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );

            }

            else
            {
                SelectedAccountProxy.Activity = new ShortMaintenanceActivity
                {
                    AssociatedActivityType =
                        typeof( ShortRegistrationActivity )
                };

                // Pro-actively lock the account proxy
                if ( !SelectedAccountProxy.Activity.ReadOnlyAccount() )
                {
                    bool blnLocked = AccountActivityService.PlaceLockOn( SelectedAccountProxy, String.Empty );

                    if ( !blnLocked )
                    {
                        Cursor = Cursors.Default;
                        SetActiveButtons( true );
                        return;
                    }
                }
                //***************************************************************************************

                IAccount account = SelectedAccountProxy;
                account.IsShortRegistered = true;
                Cursor = Cursors.Default;


                AccountView.IsShortRegAccount = true;
                account.Activity = new ShortRegistrationActivity
                {
                    AssociatedActivityType =
                        typeof( ActivatePreRegistrationActivity )
                };

                if ( ParentForm != null )
                {
                    ( (PatientAccessView)ParentForm ).LoadShortRegistrationView();
                }

                // Verify the account has not already been canceled
                if ( account.IsCanceled )
                {
                    MessageBox.Show( UIErrorMessages.ACCOUNT_CANCELED_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );

                }
                else
                {
                    // raise the ActivatePreregisteredAccountEvent
                    SearchEventAggregator aggregator = SearchEventAggregator.GetInstance();

                    aggregator.RaiseActivatePreregisteredAccountEvent( this, new LooseArgs( SelectedAccountProxy ) );
                    Dispose();
                }
            }
            SetActiveButtons( true );
            Cursor = Cursors.Default;
        }

        private bool checkForError()
        {
            var rcErrors = RuleEngine.AccountHasFailedError();
            return rcErrors;
        }

        private void CancelBackgroundWorkers()
        {
            // cancel the background worker(s) here...
            if ( null != backgroundWorker )
                backgroundWorker.CancelAsync();

            if ( null != toDoListWorker )
                toDoListWorker.CancelAsync();
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

        #endregion

        #region Properties
        #endregion

        #region Private Properties
        private AccountProxy SelectedAccountProxy
        {
            get
            {
                return i_SelectedAccountProxy;
            }
            set
            {
                i_SelectedAccountProxy = value;
            }
        }
        private IMessageDisplayStateManager MessageStateManager { get; set; }
        private IRuleEngine RuleEngine { get; set; }
        private string ActivatingTab { get; set; }
        #endregion

        #region Private Methods

        private void SetActiveTab()
        {
            int index;

            SuspendLayout();

            switch ( ActivatingTab )
            {

                case "PAIWalkinAccountCreation":
                    {
                        index = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;
                        break;
                    }
                case "Payor":
                    {
                        index = (int)PAIWalkinAccountCreationScreenIndexes.PAYORDETAILS;
                        break;
                    }
                case "Insured":
                    {
                        index = (int)PAIWalkinAccountCreationScreenIndexes.INSURED;
                        break;
                    }

                case "PrimaryInsuranceVerification":
                    {
                        index = (int)PAIWalkinAccountCreationScreenIndexes.PRIMARYINSVERIFICATION;
                        break;
                    }

                case "PrimaryAuthorization":
                    {
                        index = (int)PAIWalkinAccountCreationScreenIndexes.PRIMARYAUTHORIZATION;
                        break;
                    }

                default:
                    index = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;
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


            switch ( (PAIWalkinAccountCreationScreenIndexes)selectedScreenIndex )
            {
                case PAIWalkinAccountCreationScreenIndexes.INSURED:
                    DisplayInsuredDialogPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;



                case PAIWalkinAccountCreationScreenIndexes.PAYORDETAILS:
                    DisplayPayorDetailsDialogPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;



                case PAIWalkinAccountCreationScreenIndexes.PRIMARYINSVERIFICATION:
                    DisplayInsuranceVerificationPage( CoverageOrder.NewPrimaryCoverageOrder() );
                    break;



                case PAIWalkinAccountCreationScreenIndexes.PRIMARYAUTHORIZATION:
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
            tcViewTabPages.SelectedIndex = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;

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
            tcViewTabPages.SelectedIndex = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;

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
            tcViewTabPages.SelectedIndex = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;

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
            tcViewTabPages.SelectedIndex = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;

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

        public void BeforeSaveAccount()
        {
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void AfterSaveAccount( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            progressPanel1.Visible = false;
            progressPanel1.SendToBack();

            if ( e.Cancelled )
            {
                lvToDo.Enabled = true;
                btnRefreshToDoList.Enabled = true;

                SetActiveButtons( true );
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
                ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model_Account ) );
                Model_Account.Patient.SetPatientContextHeaderData();
                patientContextView.Model = Model_Account.Patient;
                patientContextView.Account = Model_Account;
                patientContextView.UpdateView();

                // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
                ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
            }
            HandleActivateRegistration();

            Cursor = Cursors.Default;
        }

        private void HandleActivateRegistration()
        {
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(c_singletonInstance, EventArgs.Empty);

            if ( shortActivation )
            {
                HandleActivateShortRegistration();
            }
            else
            {
                HandleActivateStandardRegistration();
            }
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

            switch ( (uint)m.Msg )
            {
                case WM_NOTIFY:
                    {
                        if ( progressPanel1.Visible )
                        {
                            const int loading = 1;
                            m.Result = (IntPtr)loading;
                            return;
                        }

                        //this needs to be initialized to avoid compiler warning 
                        //#0649 "Field 'field' is never assigned to, and will always have its default value 'value'" as an error
                        // for the struct NHDR
                        // ReSharper disable RedundantAssignment
                        var nmh = new NMHDR { hwndFrom = IntPtr.Zero, idFrom = 0, code = 0 };
                        // ReSharper restore RedundantAssignment

                        nmh = (NMHDR)m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            if ( !checkForError() )
                            {
                                Validate();
                            }

                            const int irc = 0;

                            m.Result = (IntPtr)irc;
                        }

                        break;
                    }
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
                    toDoListWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

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
                        if ( !lvToDo.Items.Contains( lvi ) )
                        {
                            lvToDo.Items.Add( lvi );
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

                        var arrayList = (ArrayList)actionWorklistMapping[la.Oid];

                        string worklistName;
                        if ( arrayList != null )
                        {
                            worklistName = (string)arrayList[1];
                        }
                        else
                        {
                            if ( kindOfVisitCode == VisitType.PREREG_PATIENT )
                            {
                                worklistName = worklists[(int)Worklist.PREREGWORKLISTID - 1] as string;
                            }
                            else if ( kindOfVisitCode == VisitType.EMERGENCY_PATIENT )
                            {
                                worklistName = worklists[(int)Worklist.EMERGENCYDEPARMENTWORKLISTID - 1] as string;
                            }
                            else
                            {
                                worklistName = worklists[(int)Worklist.POSTREGWORKLISTID - 1] as string;
                            }
                        }

                        if ( la.IsComposite )
                        {
                            var compositeAction = (CompositeAction)la;
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


                if ( tcViewTabPages.SelectedTab == tpPAIWalkinAccountCreation )
                {

                    paiWalkinAccountCreationView.Model = Model_Account;
                    paiWalkinAccountCreationView.UpdateView();

                    paiWalkinAccountCreationView.Focus();
                    SetActiveButtons( true );

                }

            }
            finally
            {
                EndUpdateForAllControlsIn( Controls );
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// UpdateViewInternal method called from UpdateView.
        /// </summary>
        private void UpdateViewInternal()
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

        private void loadRules()
        {
            if ( !blnRulesLoaded )
            {
                blnRulesLoaded = true;

                RuleEngine.LoadRules( Model_Account );
            }
        }

        #endregion

        #region Construction and Finalization

        public static PAIWalkinAccountView GetInstance()
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
                        c_singletonInstance = new PAIWalkinAccountView();
                    }
                }
            }
            c_singletonInstance.selectedToDoItemOid = -1;

            return c_singletonInstance;
        }

        public static PAIWalkinAccountView NewInstance()
        {
            if ( c_singletonInstance != null )
            {
                c_singletonInstance.Dispose();
            }
            c_singletonInstance = new PAIWalkinAccountView { selectedToDoItemOid = -1 };
            return c_singletonInstance;
        }

        private PAIWalkinAccountView()
            : this( new MessageDisplayStateManager(), Rules.RuleEngine.GetInstance() )
        {
        }

        private PAIWalkinAccountView( IMessageDisplayStateManager messageStateManager, IRuleEngine ruleEngine )
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
            btnActivateStandardRegistration.Message = "Click finish activity";
            btnRefreshToDoList.Message = "Click refresh TODO list";


            SetActiveButtons( true );
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
        private static PAIWalkinAccountView c_singletonInstance;

        private static readonly object c_syncRoot = new Object();
        private bool initialDisplay = true;
        private long selectedToDoItemOid;
        private string lastKeyPressed;
        private bool blnRulesLoaded;
        private ArrayList worklistitems;
        private PAIWalkinAccountViewPresenter paiWalkinAccountViewPresenter { get; set; }
        private readonly ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
        private static readonly ILog c_log = LogManager.GetLogger( typeof( PAIWalkinAccountView ) );
        
        private AccountProxy i_SelectedAccountProxy;
        private bool shortActivation;
        private bool SaveSuccess = false;
        public enum PAIWalkinAccountCreationScreenIndexes
        {
            PAIWALKINACCOUNTCREATION,
            INSURED,
            PAYORDETAILS, // Not a tab index - used by MSP Wizard button event
            PRIMARYINSVERIFICATION, // Not a tab index - used by MSP Wizard button event
            PRIMARYAUTHORIZATION, // Not a tab index - used by MSP Wizard button event
            REFERRINGNONSTAFFPHYSICIAN,
            ADMITTINGNONSTAFFPHYSICIAN,
        };

        #endregion

        #region Constants

        private const string RIGHTARROW = "RightArrow";
        private const string LEFTARROW = "LeftArrow";

        #endregion

        #region IAccountView Members
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
                Model_Account.Activity = new PAIWalkinOutpatientCreationActivity();
            }

            tcViewTabPages.SelectedIndex = (int)PAIWalkinAccountCreationScreenIndexes.PAIWALKINACCOUNTCREATION;

            var populationAggregator = PatientAccessViewPopulationAggregator.GetInstance();

            populationAggregator.RaiseActionSelectedEvent( this, new LooseArgs( Model_Account ) );

            UpdateView();
        }

        public void StartBenefitsResponsePollTimer()
        {

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

        public bool Over65Check { private get; set; }

        public bool MedicareOver65Checked { get; set; }

        public bool EnableInsuranceTab { private get; set; }

        public IRequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter { get; private set; }

        public IAccountView GetIInstance()
        {
            return null;
        }

        public Account Model_Account
        {
            get { return (Account)Model; }
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

        public bool IsMedicareAdvisedForPatient()
        {
            throw new NotImplementedException();
        }

        public void DisplayLocationRequiredFieldSummary()
        {
            throw new NotImplementedException();
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
                var selectedAccount = ( (LooseArgs)e ).Context as IAccount;

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

        #endregion

        #region IPAIWalkinView Members

        /// <summary>
        /// Save Account
        /// </summary>
        public void SaveAccount()
        {
            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeSaveAccount();

                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorker.DoWork += DoSaveAccount;
                backgroundWorker.RunWorkerCompleted += AfterSaveAccount;

                backgroundWorker.RunWorkerAsync();
            }
        }

        public void EnableTODO( bool enable )
        {
            lvToDo.Enabled = enable;
        }

        public void EnableCancel( bool enable )
        {
            btnCancel.Enabled = enable;
        }

        public void EnableRefreshTODOList( bool enable )
        {
            btnRefreshToDoList.Enabled = enable;
        }

        public void SetCursorDefault()
        {
            Cursor = Cursors.Default;
        }

        public void SetCursorWait()
        {
            Cursor = Cursors.WaitCursor;
        }

        public void ReEnableFinishButtonAndFusIcon()
        {
            Cursor = Cursors.Default;
            SetActiveButtons( true );
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( true );
        }

        public bool ShowRequiredFieldsDialogAsNeeded(string summary)
        {
            if ( summary != String.Empty )
            {
                requiredFieldsDialog = new RequiredFieldsDialog
                {
                    Title = "Warning for Remaining Errors",
                    HeaderText =
                        "The fields listed have values that are creating errors. Visit each field\r\n" +
                        "listed and correct the value before completing this activity.",
                    ErrorText = summary
                };
                try
                {
                    requiredFieldsDialog.ShowDialog( this );
                    Cursor = Cursors.Default;
                    RuleEngine.ClearActions();
                    RunRulesForTab();
                }
                finally
                {
                    requiredFieldsDialog.Dispose();
                }
                ReEnableFinishButtonAndFusIcon();
                return true;
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

                paiWalkinAccountCreationView.RequiredFieldsSummaryPresenter = RequiredFieldsSummaryPresenter;

                RequiredFieldsSummaryPresenter.ShowViewAsDialog( this );
                RuleEngine.ClearActions();
                RunRulesForTab();
                ReEnableFinishButtonAndFusIcon();
                return true;
            }
            return false;
        }

        public void RunRulesForTab()
        {
            if ( tcViewTabPages.SelectedTab == tpPAIWalkinAccountCreation )
            {
                RuleEngine.EvaluateRule( typeof( OnPAIWalkinAccountCreationForm ), Model_Account );
            }

        }

        public void ShowInvalidFieldsDialog( string inValidCodesSummary )
        {
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
                        RunRulesForTab();
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
                        RunRulesForTab();
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
        }

        #endregion IPAIWalkinView Members

    }
}
