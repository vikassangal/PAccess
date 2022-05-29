using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Web.ApplicationServices;
using System.Windows.Forms;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.DischargeViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;
using Application = System.Windows.Forms.Application;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// This class will eventually have all the business and non UI logic for
    /// the <see cref="PatientsAccountsView"/>. Some methods are made static temporarily to
    /// facilitate the moving of the logic from the view. These will be fixed
    /// after the move is complete.
    /// </summary>
    public class PatientAccountsViewPresenter : IPatientsAccountsPresenter
    {
        private static BackgroundWorker backgroundWorkerCopyAccount;

        public PatientAccountsViewPresenter( IPatientsAccountsView patientsAccountsView, IFacility facility,
                                            IMessageBoxAdapter messageBoxAdapter, Activity currentActivity )
        {
            View = patientsAccountsView;
            Facility = facility;
            MessageBoxAdapter = messageBoxAdapter;
            CurrentActivity = currentActivity;
        }

        private IFacility Facility { get; set; }

        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        void IPatientsAccountsPresenter.SetSearchedAccountNumber( string accountNum )
        {
            View.SetSearchedAccountNumber( accountNum );
        }

        void IPatientsAccountsPresenter.UpdateView()
        {
            View.UpdateView();
        }

        void IPatientsAccountsPresenter.BeforeWork()
        {
            View.BeforeWork();
        }

        IAccount IPatientsAccountsPresenter.GetSelectedAccount()
        {
            return View.GetSelectedAccount();
        }

        bool IPatientsAccountsPresenter.IsAnAccountSelected()
        {
            return View.IsAnAccountSelected();
        }

        DialogResult IPatientsAccountsPresenter.CollectOfflineInformation( IAccount anAccount )
        {
            return View.CollectOfflineInformation( anAccount );
        }

        void IPatientsAccountsPresenter.ProceedToPreMse( LooseArgs args )
        {
            View.ProceedToPreMse( args );
        }

        bool IPatientsAccountsPresenter.CreateAdditionalPreMseAccount()
        {
            return View.CreateAdditionalPreMseAccount();
        }
        bool IPatientsAccountsPresenter.CreateAdditionalUCPreMseAccount()
        {
            return View.CreateAdditionalUCPreMseAccount();
        }
        IAccount IPatientsAccountsPresenter.SelectedAccount
        {
            get { return View.SelectedAccount; }
            set { View.SelectedAccount = value; }
        }

        ListView IPatientsAccountsPresenter.PatientAccounts
        {
            get { return View.PatientAccounts; }
            set { View.PatientAccounts = value; }
        }

        Account IPatientsAccountsPresenter.NewAccount
        {
            get { return View.NewAccount; }
            set
            {
                View.NewAccount = value;

            }
        }

        IAccount IPatientsAccountsPresenter.NewIAccount
        {
            get { return View.NewIAccount; }
            set { View.NewIAccount = value; }
        }

        BackgroundWorker IPatientsAccountsPresenter.BackgroundWorkerRefresh
        {
            get { return View.BackgroundWorkerRefresh; }
        }

        BackgroundWorker IPatientsAccountsPresenter.BackgroundWorkerUpdate
        {
            get { return View.BackgroundWorkerUpdate; }
        }

        BackgroundWorker IPatientsAccountsPresenter.BackgroundWorkerEditMaintain
        {
            get { return View.BackgroundWorkerEditMaintain; }
        }

        BackgroundWorker IPatientsAccountsPresenter.BackgroundWorkerContinueActivity
        {
            get { return View.BackgroundWorkerContinueActivity; }
        }

        BackgroundWorker IPatientsAccountsPresenter.BackgroundWorkerActivatePreReg
        {
            get { return View.BackgroundWorkerActivatePreReg; }
        }

        BackgroundWorker IPatientsAccountsPresenter.BackgroundWorkerCompletePostMSE
        {
            get { return View.BackgroundWorkerCompletePostMSE; }
        }

        CommandButtonManager IPatientsAccountsPresenter.Cbm
        {
            get { return View.Cbm; }
        }

        public Activity CurrentActivity { get; private set; }

        private IPatientsAccountsView View { get; set; }
        private IEMPIFeatureManager EMPIFeatureManager { get; set; }
        private IEnumerable<IAccount> AccountsForSelectedPatient { get; set; }
        InterfacilityWarningPopup interfacilityWarningPopup;
        private readonly IFacilityBroker i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        #region IDisposable Members



        #endregion

        public void HandleCreateNewAccount()
        {
            
            if (CurrentActivity.GetType() == typeof(ShortRegistrationActivity) ||
                CurrentActivity.GetType() == typeof(ShortPreRegistrationActivity))
            {
                AccountView.IsShortRegAccount = true;
            }
            else
            {
                AccountView.IsShortRegAccount = false;
            }

            if ( CurrentActivity.GetType() == typeof( QuickAccountCreationActivity ) )
            {
                AccountView.IsQuickRegistered = true;
            }
            else
            {
                AccountView.IsQuickRegistered = false;
            }
            if (CurrentActivity.GetType() == typeof( PAIWalkinOutpatientCreationActivity ))
            {
                AccountView.IsPAIWalkinRegistered = true;
            }
            else
            {
                AccountView.IsPAIWalkinRegistered = false;
            }
            if ( CurrentActivity.GetType() == typeof( RegistrationActivity ) ||
                 CurrentActivity.GetType() == typeof( ShortRegistrationActivity ) )
            {
                HandleDuplicatePreRegisterations();
            }
            else
            {
                CreateNewAccount( View );
            }

            View.EnableButtonsForActivity();
        }

        private void HandleDuplicatePreRegisterations()
        {
            AccountsForSelectedPatient = GetAccountsForCurrentPatient();

            IList<IAccount> duplicatePreRegistrationAccounts = GetDuplicatePreRegistratonAccounts();

            bool duplicateAccountDetected = duplicatePreRegistrationAccounts.Count > 0;

            if ( duplicateAccountDetected )
            {
                var warningMessage = ConstructDuplicateAccountWarningMessage( duplicatePreRegistrationAccounts );

                var userDisregardsWarning = ShowDuplicateAccountWarningMessageAndGetUserResponse( warningMessage );

                if ( userDisregardsWarning )
                {
                    CreateNewAccount( View );
                }
            }

            else
            {
                CreateNewAccount( View );
            }
        }

        private IList<IAccount> GetDuplicatePreRegistratonAccounts()
        {
            var today = Facility.GetCurrentDateTime();

            var duplicatePreRegistratonAccounts = new List<IAccount>();

            foreach ( IAccount account in AccountsForSelectedPatient )
            {
                if ( account.AdmitDate.Date == today.Date && account.KindOfVisit.IsPreRegistrationPatient && !account.IsCanceledPreRegistration )
                {
                    duplicatePreRegistratonAccounts.Add( account );
                }
            }

            return duplicatePreRegistratonAccounts;
        }

        /// <summary>
        /// Constructs the duplicate account warning message. This method is
        /// made internal for testing purposes only. It is not meant for direct
        /// client usage.
        /// </summary>
        /// <param name="accounts">The accounts.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">There should be least one
        /// account in the list</exception>
        /// /// <exception cref="ArgumentNullException">When accounts is null</exception>
        internal static string ConstructDuplicateAccountWarningMessage( ICollection<IAccount> accounts )
        {
            const int numberOfMessagesToSelect = 4;

            Guard.ThrowIfArgumentIsNull( accounts, "accounts" );

            if ( accounts.Count <= 0 )
                throw new ArgumentException( "There should be least one account in the list", "accounts" );

            IList<StringBuilder> duplicateAccountNumbers = accounts
                .Take( numberOfMessagesToSelect )
                .Select( x => new StringBuilder( x.AccountNumber.ToString() ) )
                .ToList();

            duplicateAccountNumbers.ToList().ForEach( x => x.Insert( 0, "#" ) );

            string formattedAccountNumbers = String.Join( ",", duplicateAccountNumbers.Select( x => x.ToString() ).ToArray() );

            const string messageTemplateForOneAccount =
                "A pre-registered account ({0}) exists for this patient. Do you want to proceed with the current registration or cancel to use the existing account?";
            const string messageTemplateForMoreThanOneAcccount =
                "Pre-registered accounts ({0}) exist for this patient. Do you want to proceed with the current registration or cancel to use the existing account?";

            string message = String.Format( ( duplicateAccountNumbers.Count == 1 ) ? messageTemplateForOneAccount
                                                : messageTemplateForMoreThanOneAcccount, formattedAccountNumbers );

            return message;
        }

        private bool ShowDuplicateAccountWarningMessageAndGetUserResponse( string message )
        {
            var result = MessageBoxAdapter.ShowMessageBox( message, "Patient Has Existing Pre-register Account", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );
            return result == DialogResult.OK;
        }

        /// <exception cref="InvalidOperationException">No account was selected</exception>
        private IEnumerable<IAccount> GetAccountsForCurrentPatient()
        {
            if ( View.IsAnAccountSelected() )
            {
                IAccount account = View.GetSelectedAccount();
                Patient patientForSelectedAccount = account.Patient;
                return patientForSelectedAccount.Accounts.Cast<IAccount>();
            }

            return new List<IAccount>();
        }

        private void CreateNewAccount( IPatientsAccountsView view )
        {
            view.Show();

            //Copy demographics information only and display the PatientAccountView
            if ( view.CurrentActivity.GetType().Name == "PreMSERegisterActivity" )
            {
                //Check to see if we should create another account with PRE-MSE status
                if ( !view.CreateAdditionalPreMseAccount() )
                {
                    return;
                }
            }
            //Copy demographics information only and display the PatientAccountView
            if (view.CurrentActivity.GetType().Name == "UCCPreMSERegistrationActivity")
            {
                //Check to see if we should create another account with PRE-MSE status
                if (!view.CreateAdditionalUCPreMseAccount())
                {
                    return;
                }
            }
            //HandleInterfacilityPopup();
            if ( ( view.PatientAccounts.SelectedItems.Count > 0 ) && ( view.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = view.PatientAccounts.SelectedItems[0].Tag as IAccount;
                if ( account != null )
                {
                    if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
                    {
                        DialogResult result =
                            MessageBoxAdapter.Show( UIErrorMessages.PATIENT_ACCTS_CREATING_FROM_LOCKED_ACCT_1 +
                                                   UIErrorMessages.PATIENT_ACCTS_CREATING_FROM_LOCKED_ACCT_2,
                                                   "Creating Account Based on Locked Account",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                                   MessageBoxDefaultButton.Button1 );

                        if ( result == DialogResult.No )
                        {
                            return;
                        }
                    }

                    CopySelectedAccountInformation( account );
                }
            }
            else
            {
                CreateNewAccountWithDemographicsOnly();
            }

            // initialize actions loader

            // don't proceed until the background worker is finished...
            while ( view.BackgroundWorkerUpdate.IsBusy || ( backgroundWorkerCopyAccount != null && backgroundWorkerCopyAccount.IsBusy ) )
            {
                Application.DoEvents();
            }

            if ( view.NewAccount != null )
            {
                view.NewAccount.IsNew = true;

                LooseArgs args = new LooseArgs( view.NewAccount );

                switch ( view.CurrentActivity.GetType().Name )
                {
                    case "ShortRegistrationActivity":
                    case "ShortPreRegistrationActivity":
                    case "PostMSERegistrationActivity":
                    case "PreRegistrationActivity":
                    case "RegistrationActivity":
                    case "UCCPostMseRegistrationActivity":
                        ProceedToRegOrPreregAccount( args );
                        break;
                    case "PreMSERegisterActivity":
                        view.ProceedToPreMse( args );
                        break;
                    case "QuickAccountCreationActivity":
                        ProceedToRegOrPreregAccount( args );
                        break;
                    case "PAIWalkinOutpatientCreationActivity":
                        ProceedToRegOrPreregAccount(args);
                        break;
                    case "UCCPreMSERegistrationActivity":
                        view.ProceedToPreMse(args);
                        break;
                }
            }
        }
        public bool HandleInterfacilityPopup()
        {
            bool createNewAccount = true;
            if (User.GetCurrent().Facility.IsSATXEnabled)
            {
                //Popup warning Interfacility message to get From Hospital and From Account
                
                InterFacilityTransferFeatureManager interFacilityTransferFeatureManager = new InterFacilityTransferFeatureManager();
                bool IsITFREnabled =
                    interFacilityTransferFeatureManager.IsITFREnabled(User.GetCurrent().Facility, this.CurrentActivity);
                if (IsITFREnabled && this.CurrentActivity.IsQuickAccountCreationActivity())
                {
                    interfacilityWarningPopup = new InterfacilityWarningPopup();
                    interfacilityWarningPopup.Title = "Interfacility Transfer";
                    interfacilityWarningPopup.HeaderText = "Do you want to link this admission to perform a transfer from another hospital?\r\n\r\n"
                        + "If Yes: Provide 'From Hospital' & 'From Account' details and press Continue \r\n"
                        + "If No: Leave the below fields blank and press Continue \r\n\r\nClick Cancel to abort this activity";

                    interfacilityWarningPopup.TranferPatient = View.Model as Patient;
                    interfacilityWarningPopup.ShowDialog();                    
                    createNewAccount = interfacilityWarningPopup.CancelActivity;
                    (View.Model as Patient).InterFacilityTransferAccount.FromAccountNumber
                         = interfacilityWarningPopup.FromAccount.Patient.InterFacilityTransferAccount.FromAccountNumber;
                    (View.Model as Patient).InterFacilityTransferAccount.FromFacilityOid
                         = interfacilityWarningPopup.FromAccount.Patient.InterFacilityTransferAccount.FromFacilityOid;
                    if (interfacilityWarningPopup.CancelActivity)
                    {
                        (View.Model as Patient).InterFacilityTransferAccount.FromFacility =
                            i_FacilityBroker.FacilityWith(interfacilityWarningPopup.FromAccount.Patient
                                .InterFacilityTransferAccount.FromFacilityOid);
                    }
                    
                }
            }
            return createNewAccount;
        }

        private void CreateNewAccountWithDemographicsOnly()
        {
            View.NewIAccount = new Account { IsNew = true };

            // TLG 06/21/2007 extra precaution

            if ( View.Model == null )
            {
                return;
            }

            AbstractPatient abstractPatient = View.Model as AbstractPatient;

            if ( abstractPatient != null )
            {
                Patient realPatient = abstractPatient.AsPatient();
                realPatient.AddAccount( View.NewIAccount );
                View.NewIAccount.Patient = realPatient;
               
            }

            View.NewIAccount.Activity = View.CurrentActivity;

            if (View.CurrentActivity.GetType().Name != "PreRegistrationActivity" &&
                View.CurrentActivity.GetType() != typeof (QuickAccountCreationActivity) &&
                View.CurrentActivity.GetType() != typeof (PAIWalkinOutpatientCreationActivity)
                )
            {
                View.NewIAccount.AdmitDate = User.GetCurrent().Facility.GetCurrentDateTime();
            }
            View.NewIAccount.IsShortRegistered = false;
            View.NewIAccount.IsQuickRegistered = false;
            if (View.NewIAccount.Activity is ShortPreRegistrationActivity ||
                View.NewIAccount.Activity is ShortRegistrationActivity)
            {
                View.NewIAccount.IsShortRegistered = true;
            }
            else if (View.NewIAccount.Activity is QuickAccountCreationActivity)
            { 
                View.NewIAccount.IsQuickRegistered = true;
            }


            // TLG 06/19/2006
            // default the facility to user's current facility
            View.NewIAccount.Facility = User.GetCurrent().Facility;

            View.NewAccount = View.NewIAccount as Account;
            if ( View.NewAccount.Patient.PreviousName == null )
            {
                View.NewAccount.Patient.PreviousName = new Name( View.NewAccount.Patient.Name.FirstName,
                                                                View.NewAccount.Patient.Name.LastName, String.Empty );
            }

            if ( abstractPatient != null && abstractPatient.MedicalGroupIPA != null )
            {
                View.NewAccount.MedicalGroupIPA = abstractPatient.MedicalGroupIPA;
            }
            if (IsEMPIFeatureEnabled(View.NewAccount))
            {
                View.NewAccount.OverLayEMPIData();
                View.NewAccount.Patient.IsNew = true;
                View.NewAccount.Activity.EmpiPatient = null;
            }
        }
        private  bool IsEMPIFeatureEnabled(IAccount localAccount)
        {
            EMPIFeatureManager = new EMPIFeatureManager(localAccount.Facility);
            return (EMPIFeatureManager.IsEMPIFeatureEnabled(localAccount.Activity));
        }

        /// <exception cref="LoadAccountTimeoutException"><c>LoadAccountTimeoutException</c>.</exception>
        private void DoCopySelectedAccount( object sender, DoWorkEventArgs e )
        {
            try
            {
                View.NewIAccount = new Account();

                View.SelectedAccount.Activity = View.CurrentActivity;
                View.NewIAccount.Activity = View.CurrentActivity;

                View.NewIAccount = View.CurrentActivity.CreateAccountForActivityFrom( View.SelectedAccount );

                // OTD# 37233 fix - Set DischargeDate of the new 'Copy To' account to MinValue here instead of 
                // in the CreateNewAccount() method which was setting the Discharge date of the 'Copy From' 
                // selectedAccount to MinValue causing undesired results in the BillingView Occurrence Spans.
                View.NewIAccount.DischargeDate = DateTime.MinValue;
                View.NewIAccount.Patient.InterFacilityTransferAccount = View.SelectedAccount.Patient.InterFacilityTransferAccount;
                if ( View.CurrentActivity.GetType() != typeof( PreRegistrationActivity ) &&
                    View.CurrentActivity.GetType() != typeof( ShortPreRegistrationActivity ) &&
                   View.CurrentActivity.GetType() != typeof( QuickAccountCreationActivity ) &&
                   View.CurrentActivity.GetType() != typeof( PAIWalkinOutpatientCreationActivity ))
                {
                    View.NewIAccount.AdmitDate = User.GetCurrent().Facility.GetCurrentDateTime();
                }
                else
                {
                    PatientBrokerProxy patientBroker = new PatientBrokerProxy();
                    ArrayList patientTypes =
                        ( ArrayList )patientBroker.AllPatientTypes( View.SelectedAccount.Facility.Oid );

                    foreach ( VisitType patType in patientTypes )
                    {
                        if ( patType.IsPreRegistrationPatient )
                        {
                            View.NewIAccount.KindOfVisit = patType;
                            break;
                        }
                    }
                }

                // poll CancellationPending and set e.Cancel to true and return 
                if ( backgroundWorkerCopyAccount.CancellationPending )
                {
                    e.Cancel = true;
                    return;
                }

                View.NewAccount = View.NewIAccount as Account;
                if ( View.NewAccount != null && View.NewAccount.Patient.PreviousName == null )
                {
                    View.NewAccount.Patient.PreviousName = new Name( View.NewAccount.Patient.Name.FirstName,
                                                                    View.NewAccount.Patient.Name.LastName, String.Empty );
                }
            }
            catch ( RemotingTimeoutException exception )
            {
                throw new LoadAccountTimeoutException( String.Empty, exception );
            }
        }

        private static void SetCreateNewAccountButton( IPatientsAccountsPresenter view )
        {
            if ( backgroundWorkerCopyAccount == null || !backgroundWorkerCopyAccount.IsBusy )
            {
                view.Cbm.Command( "btnCreateNewAccount" ).Enabled = true;
            }
        }

        private static void SetCreateNewAccountButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {

            view.Cbm.Command("btnCreateNewAccount").Enabled = true;
            if ( accountSelected != null)
            {
                //disable the create new account button for Pre-Admit Newborn account
                if ( accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                    accountSelected.IsNewBorn )
                {
                    view.Cbm.Command( "btnCreateNewAccount" ).Enabled = false;
                }
            }
            if (accountSelected != null)
            {
                //disable the create new account button for Walkin account
                if (accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                    accountSelected.IsPAIWalkinRegistered)
                {
                    view.Cbm.Command("btnCreateNewAccount").Enabled = false;
                }
            }
            
        }
        /// <summary>
        ///
        /// Cancels all background worker threads. This method is used to cancel
        /// all background worker threads used in this class. We need a better
        /// way of doing this.
        /// </summary>
        /// <param name="view">The view.</param>
        public static void CancelAllBackgroundWorkerThreads( PatientsAccountsView view )
        {
            // cancel all cancel the background worker(s) here as a catch-all 
            // (not very elegant, but works without complications.)
            //
            if ( view.BackgroundWorkerUpdate != null )
                view.BackgroundWorkerUpdate.CancelAsync();

            if ( view.BackgroundWorkerRefresh != null )
                view.BackgroundWorkerRefresh.CancelAsync();

            if ( backgroundWorkerCopyAccount != null )
                backgroundWorkerCopyAccount.CancelAsync();

            if ( view.BackgroundWorkerEditMaintain != null )
                view.BackgroundWorkerEditMaintain.CancelAsync();

            if ( view.BackgroundWorkerActivatePreReg != null )
                view.BackgroundWorkerActivatePreReg.CancelAsync();

            
            if ( view.BackgroundWorkerCompletePostMSE != null )
                view.BackgroundWorkerCompletePostMSE.CancelAsync();

            if ( view.BackgroundWorkerContinueActivity != null )
                view.BackgroundWorkerContinueActivity.CancelAsync();
        }

        private void CopySelectedAccountInformation( IAccount account )
        {
            View.SelectedAccount = account;

            if ( backgroundWorkerCopyAccount == null ||
                ( backgroundWorkerCopyAccount != null
                 && !backgroundWorkerCopyAccount.IsBusy )
                )
            {
                View.BeforeWork();

                backgroundWorkerCopyAccount = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorkerCopyAccount.DoWork += DoCopySelectedAccount;
                backgroundWorkerCopyAccount.RunWorkerCompleted += View.AfterWork;

                backgroundWorkerCopyAccount.RunWorkerAsync();
            }
        }

        //In order to test the presenter with SelectedAccount value, change the method from static to instance method
        public void EnableButtonsForActivity( IPatientsAccountsView view )
        {
            IAccount selected = null;

            if (((IPatientsAccountsPresenter)this).SelectedAccount != null)
                selected = ((IPatientsAccountsPresenter)this).SelectedAccount;
            
            view.Cbm.DisableButtons();

            view.Cbm.Command( "btnCancel" ).Enabled = true;

            switch ( view.CurrentActivity.GetType().Name )
            {
                case "PreRegistrationActivity":
                    SetCreateNewAccountButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "QuickAccountCreationActivity":
                    SetCreateNewAccountButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "PAIWalkinOutpatientCreationActivity":
                    SetCreateNewAccountButton(view, selected);
                    SetMaintenanceButton(view, selected);
                    break;
                case "RegistrationActivity":
                    SetActivatePreregButton( view, selected );
                    SetActivatePreregShortButton(view, selected);
                    SetCreateNewAccountButton(view, selected);
                    SetMaintenanceButton( view, selected );
                    break;
                case "ShortPreRegistrationActivity":
                    SetCreateNewAccountButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "ShortRegistrationActivity":
                    SetActivatePreregButton( view, selected );
                    SetActivatePreregShortButton(view, selected);
                    SetCreateNewAccountButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "PreMSERegisterActivity":
                    SetCreateNewAccountButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "PostMSERegistrationActivity":
                    SetCompletePostMseButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "PreAdmitNewbornActivity":
                    SetCreatePreNewbornButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "AdmitNewbornActivity":
                    SetCreateNewbornButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "MaintenanceActivity":
                    SetMaintenanceButton( view, selected );
                    SetConvertShortPreregButton(view, selected);
                    break;
                case "UCCPreMSERegistrationActivity":
                    SetCreateNewAccountButton( view, selected );
                    SetMaintenanceButton( view, selected );
                    break;
                case "UCCPostMseRegistrationActivity":
                    SetCompleteUCCPostMseButton(view, selected);
                    SetMaintenanceButton( view, selected );
                    break;
                case "CancelPreRegActivity":
                case "PreDischargeActivity":
                case "DischargeActivity":
                case "EditDischargeDataActivity":
                case "EditRecurringDischargeActivity":
                case "CancelInpatientDischargeActivity":
                case "CancelOutpatientDischargeActivity":
                case "CancelInpatientStatusActivity":
                case "TransferActivity":
                case "TransferOutToInActivity":
                case "TransferInToOutActivity":
                case Activity.TransferErPatientToOutPatient:
                case Activity.TransferOutPatientToErPatient:
                    SetContinueButton( view, selected );
                    break;
                default:
                    break;
            }
        }

        public void CancelActivity()
        {
            View.Dispose();
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( View, null );
        }

        private void ProceedToRegOrPreregAccount( LooseArgs args )
        {
            IAccount anAccount = null;

            if ( args != null )
            {
                anAccount = args.Context as IAccount;
            }

            if ( View.CurrentActivity.AssociatedActivityType == typeof( PreRegistrationWithOfflineActivity )
                || View.CurrentActivity.AssociatedActivityType == typeof( RegistrationWithOfflineActivity )
                || View.CurrentActivity.AssociatedActivityType == typeof( PreMSERegistrationWithOfflineActivity )
                || View.CurrentActivity.AssociatedActivityType == typeof( AdmitNewbornWithOfflineActivity )
                || View.CurrentActivity.AssociatedActivityType == typeof( PreAdmitNewbornWithOfflineActivity )
                  || View.CurrentActivity.AssociatedActivityType == typeof(ShortPreRegistrationWithOfflineActivity)
                  || View.CurrentActivity.AssociatedActivityType == typeof(ShortRegistrationWithOfflineActivity)
                )
            {
                // launch popup to collect AccountNumber and/or MedicalRecordNumber

                DialogResult result = View.CollectOfflineInformation( anAccount );

                if ( result == DialogResult.Cancel )
                {
                    return;
                }
            }

            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( View, args );
            SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( View, args );
        }

        private static void SetContinueButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {
            bool isEnabled = false;
            AccountProxy accountProxy = ( AccountProxy )accountSelected;

            if ( accountSelected != null && !accountSelected.IsLocked )
            {
                switch ( view.CurrentActivity.GetType().Name )
                {
                    case "CancelPreRegActivity":
                        if ( accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                            accountProxy.DerivedVisitType != Account.PRE_CAN )
                        {
                            isEnabled = true;
                        }
                        break;
                    case "PreDischargeActivity":
                    case "DischargeActivity":
                        if ((accountSelected.KindOfVisit.Code == VisitType.INPATIENT ||
                             accountSelected.KindOfVisit.Code == VisitType.OUTPATIENT ||
                              accountSelected.KindOfVisit.Code == VisitType.NON_PATIENT ||
                             (accountSelected.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                              accountSelected.HospitalService.Code == HospitalService.EMERGENCY_ROOM)
                            )
                            && accountSelected.FinancialClass.Code != FinancialClass.MED_SCREEN_EXM_CODE &&
                            accountSelected.DischargeDate == DateTime.MinValue)
                        {
                            isEnabled = true;
                        }

                        break;
                    // SR39492 - Validate that the selected patient has a discharge date and is either a:
                    // a) Patient Type 2 (Outpatient) where the Daycare flag has been set to ‘Y’ (for the specified Hospital
                    //    Service Code indicating a bed assignment, such as in the case of HSV’s 58, 59, FO, LD or LB) or a
                    // b) Patient Type 3 (Outpatient - ER) Hospital Service Code 65.
                    case "CancelOutpatientDischargeActivity":
                        if ((((accountSelected.KindOfVisit.Code == VisitType.OUTPATIENT &&
                              !accountSelected.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE)) &&
                              accountSelected.HospitalService.IsDayCare() &&
                              accountSelected.HospitalService.DayCare == "Y" ) ||
                             ( accountSelected.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                              accountSelected.HospitalService.Code.Equals( HospitalService.EMERGENCY_ROOM ) ) ) &&
                            accountSelected.DischargeDate != DateTime.MinValue )
                        {
                            isEnabled = true;
                        }
                        break;
                    case "CancelInpatientDischargeActivity":
                    case "EditDischargeDataActivity":
                        if ( accountSelected.KindOfVisit.Code == VisitType.INPATIENT &&
                            accountSelected.DischargeDate != DateTime.MinValue )
                        {
                            isEnabled = true;
                        }
                        break;
                    case "EditRecurringDischargeActivity":
                        if ( accountSelected.KindOfVisit.Code == VisitType.RECURRING_PATIENT )
                        {
                            isEnabled = true;
                        }
                        break;
                    case "TransferActivity":
                        if ((accountSelected.KindOfVisit.Code == VisitType.INPATIENT ||
                             (accountSelected.KindOfVisit.Code == VisitType.OUTPATIENT &&
                             !accountSelected.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE) )||
                             accountSelected.KindOfVisit.Code == VisitType.RECURRING_PATIENT ||
                             accountSelected.KindOfVisit.Code == VisitType.NON_PATIENT) ||
                             (accountSelected.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                             (accountSelected.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE) == false)))
                        {
                            isEnabled = true;
                        }
                        break;
                    case "TransferOutToInActivity":
                        if ((accountSelected.KindOfVisit.Code == VisitType.OUTPATIENT &&
                             !accountSelected.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE)) ||
                            (  accountSelected.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT   &&
                             ( accountSelected.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) == false ) ) )
                        {
                            isEnabled = true;
                        }
                        break;
                    case "TransferInToOutActivity":
                        if ( accountSelected.KindOfVisit.Code == VisitType.INPATIENT &&
                            accountSelected.DischargeDate == DateTime.MinValue )
                        {
                            isEnabled = true;
                        }
                        break;
                    case "CancelInpatientStatusActivity":
                        if ( accountSelected.KindOfVisit.Code == VisitType.INPATIENT &&
                            accountSelected.AdmitDate.Date == User.GetCurrent().Facility.GetCurrentDateTime().Date &&
                            accountSelected.DischargeDate == DateTime.MinValue )
                        {
                            isEnabled = true;
                        }
                        break;
                    case Activity.TransferErPatientToOutPatient:
                        if  ( accountSelected.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT   &&
                             ( accountSelected.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) == false ) ) 
                        {
                            isEnabled = true;
                        }
                        break;
                    case Activity.TransferOutPatientToErPatient:
                        if ((accountSelected.KindOfVisit.Code == VisitType.OUTPATIENT &&
                             accountSelected.HospitalService.IsDayCare() &&
                             accountSelected.HospitalService.DayCare == "Y" ) &&
                             !accountSelected.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE) )
                        {
                            isEnabled = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            if ( view.BackgroundWorkerContinueActivity == null ||
                !view.BackgroundWorkerContinueActivity.IsBusy )
            {
                view.Cbm.Command( "btnContinue" ).Enabled = isEnabled;
            }
        }

        private static void SetActivatePreregButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {
            bool isEnabled = false;
            AccountProxy accountProxy = ( AccountProxy )accountSelected;

            if ( accountSelected != null && !accountSelected.IsLocked )
            {
                if ( accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                    accountProxy.DerivedVisitType != Account.PRE_PUR &&
                    accountProxy.DerivedVisitType != Account.PND_PUR &&
                    !accountSelected.IsCanceledPreRegistration &&
                    !accountSelected.IsCanceled) 
                {
                    isEnabled = true;
                }
            }

            if ( view.BackgroundWorkerActivatePreReg == null ||
                !view.BackgroundWorkerActivatePreReg.IsBusy )
            {
                view.Cbm.Command( "btnActivatePrereg" ).Enabled = isEnabled;
            }
        }

        private static void SetActivatePreregShortButton(IPatientsAccountsPresenter view, IAccount accountSelected)
        {
            bool isEnabled = false;
            AccountProxy accountProxy = (AccountProxy)accountSelected;

            if (accountSelected != null && !accountSelected.IsLocked)
            {
                if (accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                    accountProxy.DerivedVisitType != Account.PRE_PUR &&
                    accountProxy.DerivedVisitType != Account.PND_PUR &&
                    !accountSelected.IsCanceledPreRegistration &&
                    !accountSelected.IsCanceled && !accountSelected.IsNewBorn)
                {
                    isEnabled = true;
                }
            }
            
            if (view.BackgroundWorkerActivatePreReg == null ||
                !view.BackgroundWorkerActivatePreReg.IsBusy)
            {
                view.Cbm.Command("btnActivatePreregShort").Enabled = isEnabled;
            }
        }

        
        private static void SetConvertShortPreregButton(IPatientsAccountsPresenter view, IAccount accountSelected)
        {
            bool isEnabled = false;
            AccountProxy accountProxy = (AccountProxy)accountSelected;

            if (accountSelected != null && !accountSelected.IsLocked)
            {
                if (accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                    accountProxy.DerivedVisitType != Account.PRE_PUR &&
                    accountProxy.DerivedVisitType != Account.PND_PUR &&
                    accountSelected.IsShortRegistered ==false &&
                    accountSelected.IsQuickRegistered == false  &&
					!accountSelected.IsNewBorn &&
                    !accountSelected.IsCanceledPreRegistration &&
                    !accountSelected.IsCanceled)
                {
                    isEnabled = true;
                }
                if (accountSelected.IsPAIWalkinRegistered)
                {
                    isEnabled = false;
                }
            }
            //Nereid, use editmaintain background for now. Might need to add a new one for convert.
            if (view.BackgroundWorkerEditMaintain == null ||
                !view.BackgroundWorkerEditMaintain.IsBusy)
            {
                view.Cbm.Command("btnConvertToDiagPrereg").Enabled = isEnabled;
            }
        }
        private static void SetMaintenanceButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {
            bool isEnabled = false;
            AccountProxy accountProxy = ( AccountProxy )accountSelected;

            if ( accountSelected != null &&
                !accountSelected.IsLocked )
            {
                User patientAccessUser = User.GetCurrent();
                Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;

                Peradigm.Framework.Domain.Parties.Facility securityFrameworkFacility =
                    new Peradigm.Framework.Domain.Parties.Facility( patientAccessUser.Facility.Code,
                                                                   patientAccessUser.Facility.Description );
                bool hasEditPermission = securityUser.HasPermissionTo( Privilege.Actions.Edit,
                                                                      accountProxy.KindOfVisit.Code,
                                                                      securityFrameworkFacility );

                if ( hasEditPermission )
                {
                    isEnabled = true;
                }
                if (accountSelected.IsPAIWalkinRegistered)
                {
                    isEnabled = false;
                }
            }

            // should not be able to start another thread if the old one not return
            if ( view.BackgroundWorkerEditMaintain == null ||
                !view.BackgroundWorkerEditMaintain.IsBusy )
            {
                view.Cbm.Command( "btnEditAccount" ).Enabled = isEnabled;
            }
        }

        private static void SetCreateNewbornButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {
            if ( accountSelected != null &&
                !accountSelected.IsLocked &&
                accountSelected.KindOfVisit.Code == VisitType.INPATIENT &&
                accountSelected.HospitalService.Code == "16" &&
                accountSelected.DischargeDate == DateTime.MinValue &&
                accountSelected.Patient.Sex.Code == Gender.FEMALE_CODE )
            {
                view.Cbm.Command( "btnCreateNewbornAccount" ).Enabled = true;
            }
            else
            {
                view.Cbm.Command( "btnCreateNewbornAccount" ).Enabled = false;
            }
        }

        private static void SetCreatePreNewbornButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {
            //SR 1557 Mother can be either 1: PT=1, HSV=16, Female, not discharged
            //                          or 2: PT=0, HSV=35, not canceledPrereg, Female, AdmitDate is within 90 days from today( not consider Admit Time), and not QAC account     
            if ( accountSelected != null &&
                !accountSelected.IsLocked &&
                    ((accountSelected.KindOfVisit.Code == VisitType.INPATIENT &&
                    accountSelected.HospitalService.Code == "16" &&
                    accountSelected.DischargeDate == DateTime.MinValue &&
                    accountSelected.Patient.Sex.Code == Gender.FEMALE_CODE) ||
                        (accountSelected.KindOfVisit.Code == VisitType.PREREG_PATIENT && 
                            !accountSelected.IsQuickRegistered && 
                                !accountSelected.IsPAIWalkinRegistered &&
                        accountSelected.HospitalService.Code == "35" && !accountSelected.IsCanceledPreRegistration && !accountSelected.IsCanceled &&
                        accountSelected.Patient.Sex.Code == Gender.FEMALE_CODE &&
                        accountSelected.AdmitDate.Date >= User.GetCurrent().Facility.GetCurrentDateTime().Date &&
                        accountSelected.AdmitDate.Date <= User.GetCurrent().Facility.GetCurrentDateTime().Date.AddDays(90) 
                        )   
                   )
               )
            {
                view.Cbm.Command( "btnCreatePreNewbornAccount" ).Enabled = true;
            }
            else
            {
                view.Cbm.Command( "btnCreatePreNewbornAccount" ).Enabled = false;
            }
        }

        private static void SetCompletePostMseButton( IPatientsAccountsPresenter view, IAccount accountSelected )
        {
            bool isEnabled = false;

            if ( accountSelected != null &&
                !accountSelected.IsLocked &&
                ( accountSelected.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT  ) &&
                accountSelected.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) )
            {
                isEnabled = true;
            }

            if ( view.BackgroundWorkerCompletePostMSE == null ||
                !view.BackgroundWorkerCompletePostMSE.IsBusy )
            {
                view.Cbm.Command( "btnPostMSE" ).Enabled = isEnabled;
            }
        }
        private static void SetCompleteUCCPostMseButton(IPatientsAccountsPresenter view, IAccount accountSelected)
        {
            bool isEnabled = false;

            if (accountSelected != null &&
                !accountSelected.IsLocked &&
                accountSelected.KindOfVisit.Code == VisitType.OUTPATIENT &&
                accountSelected.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE))
            {
                isEnabled = true;
            }

            if (view.BackgroundWorkerCompletePostMSE == null ||
                !view.BackgroundWorkerCompletePostMSE.IsBusy)
            {
                view.Cbm.Command("btnUCCPostMSE").Enabled = isEnabled;
            }
        }
    }
}