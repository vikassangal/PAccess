using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.UI.Registration;
using log4net;
using PatientAccess.Rules;
using PatientAccess.UI.InterfacilityTransfer;
using PatientAccess.Domain.InterFacilityTransfer;
using System.Linq;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// Business logic from this view is being moved to the <see cref="PatientAccountsViewPresenter"/>
    /// since is being done in increments some fields are made public temporarily to facilitate the move.
    /// </summary>
    public class PatientsAccountsView : ControlView, IPatientsAccountsView
    {
        #region Event Handlers

        private void PatientsAccountsView_Load( object sender, EventArgs e )
        {
            this.progressPanel1.Visible = false;
            this.lblNoVisits.Visible = false;

            this.btnRefresh.Enabled = false;
            this.btnReturnToSearch.Enabled = false;

            // TLG 01/12/07 Moved from Initialize component so that the Designer could open this view
            this.Cbm.ButtonClicked += this.cbm_ButtonClicked;
        }

        private void btnRefresh_Click( object sender, EventArgs e )
        {
            this.Cursor = Cursors.WaitCursor;

            this.btnRefresh.Enabled = false;
            this.btnReturnToSearch.Enabled = false;

            this.RefreshPatientAccountsList();

            this.Cursor = Cursors.Arrow;
        }

        private void activityContextView1_FilterOff( object sender, EventArgs e )
        {
            IsFilterOn = false;

            if ( this.activityContextView1.RadioButtonsEnabled && this.activityContextView1.OffRadioButtonChecked )
            {
                this.DisplayAccounts();
            }
        }

        private void activityContextView1_FilterOn( object sender, EventArgs e )
        {
            IsFilterOn = true;

            if ( this.activityContextView1.RadioButtonsEnabled && this.activityContextView1.OnRadioButtonChecked )
            {
                this.DisplayAccounts();
            }
            this.EnableButtonsForActivity();
        }

        private void cbm_ButtonClicked( object sender, EventArgs e )
        {
            this.progressPanel1.Visible = false;

            this.Cursor = Cursors.WaitCursor;
            this.ClickedButton = (LoggingButton)sender;

            if ( ClickedButton.Name != "btnCancel" )
            {
                this.CheckButtonClicked( ClickedButton.Name );
            }
            else
            {
                Presenter.CancelActivity();
            }

            this.Cursor = Cursors.Default;
        }

        private void btnReturnToSearch_Click( object sender, EventArgs e )
        {
            MasterPatientIndexView mpiView = this.Parent as MasterPatientIndexView;
            if ( mpiView != null )
            {
                this.btnRefresh.Enabled = false;
                this.btnReturnToSearch.Enabled = false;
                mpiView.OnReturnToSearch( this, e );
            }
        }

        private void patientAccounts_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                this.SelectedAccount = account;
                if ( account != null )
                {
                    BreadCrumbLogger.GetInstance.Log( account.AccountNumber + " account selected" );
                    this.panelButtons.Visible = false;
                    this.EnableButtonsForActivity();
                    this.SetDefaultAcceptButton();

                    if ( this.panelButtons != null )
                    {
                        this.panelButtons.Visible = true;
                    }
                }
            }
            else
            {
                this.DisableButtons();
            }
        }

        private void patientAccounts_DoubleClick( object sender, EventArgs e )
        {
            this.ProcessAccountDoubleClick();
        }
        #endregion

        #region Methods

        public void SetSearchedAccountNumber( string accountNum )
        {
            i_SearchedAccountNumber = accountNum;
        }

        private void DoUpdateViewFromSearchResult( object sender, DoWorkEventArgs e )
        {
            patient = this.Model as Patient;
            if ( patient != null )
            {
                filter = new AccountFilter( patient.Accounts );
            }

            if ( this.BackgroundWorkerUpdate.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        public override void UpdateView()
        {
            this.btnRefresh.Enabled = false;
            this.btnReturnToSearch.Enabled = false;

            Cbm.RemoveButtonsFromPanel( this.panelButtons );
            Cbm.SetPanel( this.panelButtons );

            base.UpdateView();
            EnableThemesOn( this.panelButtons );

            if ( this.BackgroundWorkerUpdate == null
                ||
                ( this.BackgroundWorkerUpdate != null
                    && !this.BackgroundWorkerUpdate.IsBusy )
              )
            {
                this.BeforeWork();

                BackgroundWorkerUpdate = new BackgroundWorker();
                BackgroundWorkerUpdate.WorkerSupportsCancellation = true;

                BackgroundWorkerUpdate.DoWork += this.DoUpdateViewFromSearchResult;

                BackgroundWorkerUpdate.RunWorkerCompleted += this.AfterWork;

                BackgroundWorkerUpdate.RunWorkerAsync();
            }
        }

        #endregion

        #region Private Methods

        private int GetMatchingAccountIndex()
        {
            //If the SearchedAccountNumber does not have a value this will return 0
            //else it will return the row index with the matching account number.

            int result = 0;
            int accountColumnIndex = GetAccountNumberColumnIndex();

            if ( i_SearchedAccountNumber != String.Empty )
            {
                for ( int i = 0; i < PatientAccounts.Items.Count; i++ )
                {
                    if ( PatientAccounts.Items[i].SubItems[accountColumnIndex].Text == i_SearchedAccountNumber )
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }

        private int GetAccountNumberColumnIndex()
        {
            int result = 0;
            for ( int i = 0; i < PatientAccounts.Columns.Count; i++ )
            {
                if ( PatientAccounts.Columns[i] == chAccount )
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        private void SetDefaultAcceptButton()
        {
            if ( Cbm.Command( "btnContinue" ) != null && this.Cbm.Command( "btnContinue" ).Enabled )
            {
                this.AcceptButton = Cbm.Command( "btnContinue" );
                Cbm.Command( "btnContinue" ).Focus();
            }
            else if ( Cbm.Command( "btnEditAccount" ) != null && this.Cbm.Command( "btnEditAccount" ).Enabled )
            {
                this.AcceptButton = Cbm.Command( "btnEditAccount" );
                Cbm.Command( "btnEditAccount" ).Focus();
            }
            else
            {
                this.AcceptButton = this.btnReturnToSearch;
                this.btnReturnToSearch.Focus();
            }
        }

        private void ProcessAccountDoubleClick()
        {
            // defect 34921 - added null check
            if ( this.Cbm != null )
            {
                if ( Cbm.Command( "btnContinue" ) != null )
                {
                    if ( Cbm.Command( "btnContinue" ).Enabled )
                    {
                        this.ClickedButton = this.Cbm.Command( "btnContinue" );
                        this.ClickedButton.Enabled = false;
                        this.ContinueActivity();
                    }
                }
                else
                {
                    if ( Cbm.Command( "btnEditAccount" ).Enabled )
                    {
                        this.ClickedButton = this.Cbm.Command( "btnEditAccount" );
                        this.ClickedButton.Enabled = false;
                        this.EditMaintainAccount();
                    }
                }
            }
        }

        private void DisableButtons()
        {
            this.Cbm.DisableButtons();
        }

        public void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            this.DisableButtons();

            this.lblNoVisits.Visible = false;

            this.pnlProgress.Visible = true;
            this.progressPanel1.Visible = true;
            this.pnlProgress.BringToFront();
            this.progressPanel1.Refresh();
        }

        private void DisplayAccounts()
        {
            if ( this.PatientAccounts.Items.Count > 0 )
            {
                this.PatientAccounts.Items[0].Selected = true;
            }

            PatientAccounts.Items.Clear();

            if ( filter != null )
            {
                filteredResults = filter.ExecuteFilter( GetCriteria() );
            }
            AbstractPatient abstractPatient = this.Model as AbstractPatient;
            //GetInterFacilityTransferAccounts(abstractPatient.MedicalRecordNumber, abstractPatient.Facility.Code);
            this.BindResults();

            if ( patient == null || this.PatientAccounts.Items.Count <= 0 )
            {
                this.PatientAccounts.Visible = false;
                this.lblNoVisits.Visible = true;
                this.btnReturnToSearch.Focus();
                ((IPatientsAccountsPresenter) this.Presenter).SelectedAccount = null;
                this.DisplayNoAccountsFoundMsg();
                return;
            }
            else
            {
                this.PatientAccounts.Items[GetMatchingAccountIndex()].Selected = true;
                this.lblNoVisits.Visible = false;
                this.PatientAccounts.Visible = true;
            }

            ttUserName.AutoPopDelay = 50;
            this.PatientAccounts.Focus();
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                this.btnRefresh.Enabled = false;
                this.btnReturnToSearch.Enabled = false;
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if ( e.Error.GetType() == typeof( RemotingTimeoutException ) )
                {
                    MessageBox.Show( UIErrorMessages.TIMEOUT_GENERAL );
                    this.lblNoVisits.Visible = false;
                    this.PatientAccounts.Clear();
                    DisableButtons();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success

                // Refactored the existing logic into a new private method called AfterWorkSuccessLogic(),
                // this was so that the AfterWork method could be easily understood, as well as the 
                // refactored logic.
                AfterWorkSuccessLogic();

                this.EnableButtonsForActivity();
            }

            // post completion operations
            this.Cursor = Cursors.Default;

            this.btnRefresh.Enabled = true;
            this.btnReturnToSearch.Enabled = true;

            this.pnlProgress.Visible = false;
            this.pnlProgress.SendToBack();
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
        }

        private void AfterWorkSuccessLogic()
        {
            this.DisplayPatient();

            if ( this.PatientAccounts != null
                && this.PatientAccounts.Items.Count > 0 )
            {
                this.PatientAccounts.SelectedIndexChanged -= this.patientAccounts_SelectedIndexChanged;
                this.PatientAccounts.Items[GetMatchingAccountIndex()].Selected = true;
                this.SetDefaultAcceptButton();
                this.PatientAccounts.SelectedIndexChanged += this.patientAccounts_SelectedIndexChanged;
            }

            this.DisplayActivity();

            if ( !this.activityContextView1.RadioButtonsEnabled )
            {
                this.DisplayAccounts();
            }
            if (this.PatientAccounts.Columns.Count != 0)
            {
                PatientAccounts.Columns.Clear();
            }

            if (CurrentActivity.IsValidRegistrationActivityForDischargeDisposition)
            {
                if (this.PatientAccounts.Columns.Count <= 0)
                {
                    this.PatientAccounts.Columns.AddRange(new[]
                    { 
                        
                        this.chBmp,
                        //this.chIfxfr,
                        this.chAdmitDate,
                        this.chDishDate,
                        this.chDishargeDisposition,
                        this.chPatientType,
                        this.chVisitType,
                        this.chHospitalService,
                        this.chClinic,
                        this.chFinancialClass,
                        this.chAccount
                    });
                    chAdmitDate.Width = 110;
                    chDishDate.Width = 85;
                    chDishargeDisposition.Width = 90;
                    chPatientType.Width = 90;
                    chVisitType.Width = 80;
                    chHospitalService.Width = 140;
                    chClinic.Width = 145;
                    chFinancialClass.Width = 160;
                    //chIfxfr.Width = 50;
                }
            }
            else
            {
                if (this.PatientAccounts.Columns.Count <= 0)
                {
                   this.PatientAccounts.Columns.AddRange(new[]
                    {
                        
                        this.chBmp,
                        //this.chIfxfr,
                        this.chAdmitDate,
                        this.chDishDate,
                        this.chPatientType,
                        this.chVisitType,
                        this.chHospitalService,
                        this.chClinic,
                        this.chFinancialClass,
                        this.chAccount
                    });
                }
            }

            if ( this.activityContextView1.Enabled )
            {
                this.activityContextView1.Focus();
            }
            else
            {
                this.PatientAccounts.Focus();
            }

            this.activityContextView1.TabStop = this.activityContextView1.RadioButtonsEnabled;

            if ( this.PatientAccounts.Items.Count == 0 )
            {
                this.btnReturnToSearch.Focus();
            }
            else
            {
                this.PatientAccounts.Focus();
            }

            this.panel1.BringToFront();
            this.panelButtons.BringToFront();
        }

        private void DoRefresh( object sender, DoWorkEventArgs e )
        {

            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            Patient thePatient = this.Model as Patient ?? patient;

            PatientSearchResult patientSearchResult = new PatientSearchResult(thePatient.Name, thePatient.Sex, thePatient.DateOfBirth,
                                                        thePatient.SocialSecurityNumber.UnformattedSocialSecurityNumber,
                                                        thePatient.MedicalRecordNumber, null,
                                                        thePatient.Facility.Code);

            patient = patientBroker.PatientFrom( patientSearchResult );

            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.BackgroundWorkerRefresh.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            filter = new AccountFilter( patient.Accounts );

            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.BackgroundWorkerRefresh.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }


        private void RefreshPatientAccountsList()
        {
            base.UpdateView();
            EnableThemesOn( this.panelButtons );

            if ( this.BackgroundWorkerRefresh == null
                ||
                ( this.BackgroundWorkerRefresh != null
                    && !this.BackgroundWorkerRefresh.IsBusy )
              )
            {
                this.BeforeWork();

                this.BackgroundWorkerRefresh = new BackgroundWorker();
                this.BackgroundWorkerRefresh.WorkerSupportsCancellation = true;

                BackgroundWorkerRefresh.DoWork += this.DoRefresh;

                BackgroundWorkerRefresh.RunWorkerCompleted += this.AfterWork;

                BackgroundWorkerRefresh.RunWorkerAsync();
            }
        }


        private void DisplayNoAccountsFoundMsg()
        {
            this.PatientAccounts.Visible = false;
            this.lblNoVisits.Visible = true;

            switch ( CurrentActivity.GetType().Name )
            {
                case "PreRegistrationActivity":
                case "RegistrationActivity":
                case "PreMSERegisterActivity":
                    this.lblNoVisits.Text = UIErrorMessages.PATIENT_ACCTS_VIEW_NO_ACCTS;
                    break;
                default:
                    if ( IsFilterOn )
                    {
                        this.lblNoVisits.Text = UIErrorMessages.PATIENT_ACCTS_VIEW_NO_ACCTS_WITH_FITLER;
                    }
                    else
                    {
                        this.lblNoVisits.Text = UIErrorMessages.PATIENT_ACCTS_VIEW_NO_ACCTS_NO_FILTER;
                    }
                    break;
            }
        }

        /// <summary>
        /// Enables the buttons for activity. This method currently forwards
        /// calls to the presenter. When the migration to the passive view
        /// pattern is complete for this view it can be removed.
        /// </summary>
        public void EnableButtonsForActivity()
        {
            this.Presenter.EnableButtonsForActivity(this);
        }


        private void CheckButtonClicked( string buttonName )
        {
            switch ( buttonName )
            {
                case "btnActivatePrereg":
                    HandleInterfacilityAcccountActivatePopup();                    
                    break;
                case "btnActivatePreregShort":
                    HandleInterfacilityAcccountActivateShortPopup();
                    break;
                case "btnConvertToDiagPrereg":
                    this.ConvertToShortPrereg(); 
                    break;
                case "btnCreateNewAccount":
                    if (Presenter.HandleInterfacilityPopup())
                    {
                        Presenter.HandleCreateNewAccount();
                    }
                    break;
                case "btnEditAccount":
                    this.EditMaintainAccount();
                    break;
                case "btnCancel":
                    Presenter.CancelActivity();
                    break;
                case "btnCreateNewbornAccount":
                    this.CreateNewbornAccount();
                    break;
                case "btnCreatePreNewbornAccount":
                    this.CreatePreNewbornAccount();
                    break;
                case "btnContinue":
                    this.ContinueActivity();
                    break;
                case "btnPostMSE":
                    this.CompletePostMseRegistration();
                    break;
                case "btnUCCPostMSE":
                    this.CompleteUCCPostMseRegistration();
                    break;
                default:
                    MessageBox.Show( "Activity Not Selected!" );
                    break;
            }
        }

        public IAccount GetSelectedAccount()
        {
            return (IAccount)this.PatientAccounts.SelectedItems[0].Tag;
        }

        public bool IsAnAccountSelected()
        {
            return ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null );
        }

        public DialogResult CollectOfflineInformation( IAccount anAccount )
        {
            this.enterOfflineInfo = new EnterOfflineInfo();
            this.enterOfflineInfo.Model_Patient = null;
            this.enterOfflineInfo.Model_IAccount = anAccount;
            this.enterOfflineInfo.UpdateView();
            return this.enterOfflineInfo.ShowDialog( this );
        }

        public void ProceedToPreMse( LooseArgs args )
        {
            IAccount anAccount = null;

            if ( args != null )
            {
                anAccount = args.Context as IAccount;
            }

            if ( this.CurrentActivity.AssociatedActivityType == typeof( PreMSERegistrationWithOfflineActivity ) )
            {
                // launch popup to collect AccountNumber and/or MedicalRecordNumber

                this.enterOfflineInfo = new EnterOfflineInfo();
                this.enterOfflineInfo.Model_Patient = null;
                this.enterOfflineInfo.Model_IAccount = anAccount;
                this.enterOfflineInfo.UpdateView();
                DialogResult result = this.enterOfflineInfo.ShowDialog( this );

                if ( result == DialogResult.Cancel )
                {
                    return;
                }
            }


            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );

            //TODO:Verify that this will go to Pre-MSE
            SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );
        }
     //*****************
        public void ProceedToQACReg( LooseArgs args )
        {
            IAccount anAccount = null;

            if ( args != null )
            {
                anAccount = args.Context as IAccount;
            }

            if ( this.CurrentActivity.AssociatedActivityType == typeof( QACRegistrationWithOfflineActivity ) )
            {
                // launch popup to collect AccountNumber and/or MedicalRecordNumber

                this.enterOfflineInfo = new EnterOfflineInfo();
                this.enterOfflineInfo.Model_Patient = null;
                this.enterOfflineInfo.Model_IAccount = anAccount;
                this.enterOfflineInfo.UpdateView();
                DialogResult result = this.enterOfflineInfo.ShowDialog( this );

                if ( result == DialogResult.Cancel )
                {
                    return;
                }
            }


            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );

            //TODO:Verify that this will go to Pre-MSE
            SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );
        }
        //*************************
        private bool PreMseAccountExist()
        {
            bool isPreMse = false;

            foreach ( ListViewItem lvi in this.PatientAccounts.Items )
            {
                IAccount account = lvi.Tag as IAccount;
                VisitType patientType = account.KindOfVisit;
                if ( account != null && account.FinancialClass.IsMedScreenExam() 
                    && patientType.IsEmergencyPatient )
                {
                    isPreMse = true;
                    break;
                }
            }
            return isPreMse;
        }
        private bool UCPreMseAccountExist()
        {
            bool isUCPreMse = false;

            foreach (ListViewItem lvi in this.PatientAccounts.Items)
            {
                IAccount account = lvi.Tag as IAccount;
                VisitType patientType = account.KindOfVisit;
                if (account != null && account.FinancialClass.IsMedScreenExam()
                    && patientType.IsOutpatient)
                {
                    isUCPreMse = true;
                    break;
                }
            }
            return isUCPreMse;
        }
        public bool CreateAdditionalPreMseAccount()
        {
            bool status = false;

            if ( !this.PreMseAccountExist() )
            {
                status = true;
            }
            else
            {
                DialogResult result = MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_EXISTING_PRE_MSE_MSG,
                                                       "Patient Has Existing Pre-MSE ED Account",
                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                                       MessageBoxDefaultButton.Button1 );

                if ( result == DialogResult.Yes )
                {
                    status = true;
                }
            }

            return status;
        }
        public bool CreateAdditionalUCPreMseAccount()
        {
            bool status = false;

            if (!this.UCPreMseAccountExist())
            {
                status = true;
            }
            else
            {
                DialogResult result = MessageBox.Show(UIErrorMessages.PATIENT_ACCTS_EXISTING_UC_PRE_MSE_MSG,
                                                       "Patient Has Existing Urgent Care Pre-MSE Account",
                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                                       MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    status = true;
                }
            }

            return status;
        }

        private void DoConvertToShortPrereg(object sender, DoWorkEventArgs e)
        {
            AccountProxy account = this.Model as AccountProxy;
            account.Activity = new MaintenanceActivity();
            account.Activity.AssociatedActivityType = this.CurrentActivity.GetType();

            if (AccountLockStatus.IsAccountLocked(account, User.GetCurrent()))
            {
                AccountActivityService.DisplayAccountLockedMsg();
                e.Cancel = true;
                return;
            }
            else
            {
                if (!account.Activity.ReadOnlyAccount())
                {
                    if (!AccountActivityService.PlaceLockOn(account, String.Empty))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.BackgroundWorkerEditMaintain.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterConvertToShortPrereg(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
            {
                ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent(this, EventArgs.Empty);
                return;
            }
            AccountProxy account = this.Model as AccountProxy;

            if (e.Cancelled)
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                this.RefreshPatientAccountsList();
                // if this is a clickOnce button, re-enable

                if (this.ClickedButton != null && this.ClickedButton.GetType() == typeof(ClickOnceLoggingButton))
                {
                    this.ClickedButton.Enabled = true;
                }
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success
                if (account == null) return;
                    if (account.IsShortRegisteredNonDayCareAccount())
                    {
                        // Setting this property on AccountView will direct the application to the 
                        // 8-tab view for a Short-Registered account instead of the regular 12-tab view
                        AccountView.IsShortRegAccount = true;
                        account.IsShortRegistered = true;
                        //Currently we don't have a separate activity for ConvertToShortPrereg. 
                        //So we use Activity=ShortPreRegistrationActivity and AssociatatedActivityType =typeof( MaintenanceActivity ) 
                        //to indicate this is a ConvertToShortPrereg Activity.
                        account.Activity = new ShortPreRegistrationActivity();
                        CurrentActivity =  new ShortPreRegistrationActivity();
                        account.Activity.AssociatedActivityType = typeof( MaintenanceActivity ); //typeof(PreRegistrationActivity);
                    }
                    if ( account.IsQuickPreRegAccount() )
                    {
                        
                        AccountView.IsQuickRegistered = true;
                        account.IsQuickRegistered = true;
                        //Currently we don't have a separate activity for ConvertToShortPrereg. 
                        //So we use Activity=ShortPreRegistrationActivity and AssociatatedActivityType =typeof( MaintenanceActivity ) 
                        //to indicate this is a ConvertToShortPrereg Activity.
                        account.Activity = new QuickAccountCreationActivity();
                        CurrentActivity = new QuickAccountCreationActivity();
                        account.Activity.AssociatedActivityType = typeof( MaintenanceActivity ); //typeof(QuickAccountCreationActivity);
                    }
                    if (account.IsPAIWalkinRegisteredAccount())
                    {

                        AccountView.IsPAIWalkinRegistered = true;
                        account.IsPAIWalkinRegistered = true;
                        account.Activity = new PAIWalkinOutpatientCreationActivity();
                        CurrentActivity = new PAIWalkinOutpatientCreationActivity();
                        account.Activity.AssociatedActivityType = typeof(MaintenanceActivity); //typeof(PAIWalkinOutpatientCreationActivity);
                    }
                    if (this.Parent != null
                        && this.Parent.GetType() == typeof(MasterPatientIndexView))
                    {
                        ((MasterPatientIndexView)this.Parent).RemoveSearch();
                    }
                LooseArgs args = new LooseArgs(account);
                SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent(this, args);
                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this, args);
                this.Hide();
            }

            this.pnlProgress.Visible = false;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
        }

        private void DoEditMaintainAccount( object sender, DoWorkEventArgs e )
        {
            AccountProxy account = this.Model as AccountProxy;
            account.Activity = new MaintenanceActivity();
            account.Activity.AssociatedActivityType = this.CurrentActivity.GetType();

            if ( ( account.KindOfVisit != null
                && account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT
                && account.FinancialClass != null
                && account.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE )
                )
            {
                account.Activity.AssociatedActivityType = typeof( PreMSERegisterActivity );
            }
            if ((account.KindOfVisit != null
               && account.KindOfVisit.Code == VisitType.OUTPATIENT
               && account.FinancialClass != null
               && account.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE
              )
               )
            {
                account.Activity.AssociatedActivityType = typeof(UCCPreMSERegistrationActivity);
            }

            if ( account.IsNewBorn && account.KindOfVisit != null && account.KindOfVisit.Code == VisitType.PREREG_PATIENT )
            {
                account.Activity.AssociatedActivityType = typeof(PreAdmitNewbornActivity);
            }
            if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
            {
                AccountActivityService.DisplayAccountLockedMsg();
                e.Cancel = true;
                // if this is a clickOnce button, re-enable

                #region Dean Bortell Defect 34743 31 July 2007
                //in AfterEditMaintainAccount ln1306, the e.Cancel is 
                //checked and if it is true the following code is done anyway
                //this code causes the exception and is unnecessary so I commented
                //it.  Testing should remove it upon verification of functionality

                #endregion

                return;
            }
            else
            {
                if ( !account.Activity.ReadOnlyAccount() )
                {
                    if ( !AccountActivityService.PlaceLockOn( account, String.Empty ) )
                    {
                        e.Cancel = true;
                        // if this is a clickOnce button, re-enable
                        #region Dean Bortell Defect 34741 31 July 2007
                        //in AfterEditMaintainAccount ln1306, the e.Cancel is 
                        //checked and if it is true the following code is done anyway
                        //this code causes the exception and is unnecessary so I commented
                        //it.  Testing should remove it upon verification of functionality

                        #endregion
                        return;
                    }
                }
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.BackgroundWorkerEditMaintain.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterEditMaintainAccount( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
            {
                ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent( this, EventArgs.Empty );
                return;
            }
            AccountProxy account = this.Model as AccountProxy;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                this.RefreshPatientAccountsList();
                // if this is a clickOnce button, re-enable

                if ( this.ClickedButton != null && this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                {
                    this.ClickedButton.Enabled = true;
                }
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success

                if ( account == null ) return;

                if ( ( account.KindOfVisit != null && 
                       account.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT && 
                       account.FinancialClass != null && 
                       account.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE )
                   )
                {
                    ( (PatientAccessView)this.ParentForm ).LoadEditPreMSEView();

                    account.Activity.AssociatedActivityType = typeof( PreMSERegisterActivity );
                }
                else if ((account.KindOfVisit != null &&
                          account.KindOfVisit.Code == VisitType.OUTPATIENT &&
                          account.FinancialClass != null &&
                          account.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE )
                    )
                {
                    ((PatientAccessView) this.ParentForm).LoadEditUCCPreMSEView();

                    account.Activity.AssociatedActivityType = typeof (UCCPreMSERegistrationActivity);
                }
                else
                {
                    if ( account.IsShortRegisteredNonDayCareAccount() )
                    {
                        // Setting this property on AccountView will direct the application to the 
                        // 8-tab view for a Short-Registered account instead of the regular 12-tab view
                        AccountView.IsShortRegAccount = true;
                        AccountView.IsQuickRegistered = false;
                        AccountView.IsPAIWalkinRegistered = false;
                        account.Activity = new ShortMaintenanceActivity();
                        CurrentActivity = new ShortMaintenanceActivity();
                        ((PatientAccessView)ParentForm).LoadMaintenanceCmdView();
                    }
                    else if (account.IsQuickPreRegAccount())
                    {
                        AccountView.IsQuickRegistered = true;
                        AccountView.IsShortRegAccount = false;
                        AccountView.IsPAIWalkinRegistered = false;
                        account.Activity = new QuickAccountMaintenanceActivity();
                        CurrentActivity = new QuickAccountMaintenanceActivity();
                        ( (PatientAccessView)ParentForm ).LoadMaintenanceCmdView();
                    }
                    else if (account.IsPAIWalkinRegisteredAccount())
                    {
                        AccountView.IsQuickRegistered = false;
                        AccountView.IsShortRegAccount = false;
                        AccountView.IsPAIWalkinRegistered = true;
                        ((PatientAccessView)ParentForm).LoadMaintenanceCmdView();
                    }
                    else if ( account.IsNewBorn && account.KindOfVisit != null && account.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                    {
                        AccountView.IsShortRegAccount = false;
                        AccountView.IsQuickRegistered = false;
                        AccountView.IsPAIWalkinRegistered = false;
                        account.Activity.AssociatedActivityType = typeof (PreAdmitNewbornActivity);

                        ((PatientAccessView) ParentForm).LoadMaintenanceCmdView();
                    }
                    else if (this.CurrentActivity.GetType() == typeof (PreMSERegisterActivity))
                    {
                        ((PatientAccessView) this.ParentForm).LoadMaintenanceCmdView();
                    }
                    else if (this.CurrentActivity.GetType() == typeof(UCCPreMSERegistrationActivity))
                    {
                        ((PatientAccessView)this.ParentForm).LoadMaintenanceCmdView();
                    }
                    else
                    {
                        AccountView.IsShortRegAccount = false;
                        AccountView.IsQuickRegistered = false;
                        AccountView.IsPAIWalkinRegistered = false;
                        account.Activity = new MaintenanceActivity();
                        CurrentActivity = new MaintenanceActivity();
                    }

                    if ( this.Parent != null
                        && this.Parent.GetType() == typeof( MasterPatientIndexView ) )
                    {
                        ( (MasterPatientIndexView)this.Parent ).RemoveSearch();
                    }
                }

                LooseArgs args = new LooseArgs( account );

                SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );
                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );

                this.Hide();
            }

            this.pnlProgress.Visible = false;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
        }

        private void EditMaintainAccount()
        {
            this.lblNoVisits.Visible = false;
            this.pnlProgress.Visible = true;
            this.progressPanel1.Visible = true;
            this.pnlProgress.BringToFront();
            this.progressPanel1.BringToFront();
            this.progressPanel1.Refresh();

            Logger.Debug( "In EditMaintainAccount..." );

            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                if (account != null && account.Patient != null)
                {
                    account.Patient.SetPatientContextHeaderData();
                }
                this.Model = account;

                if ( account != null )
                {
                    if ( this.BackgroundWorkerEditMaintain == null ||
                         (
                            this.BackgroundWorkerEditMaintain != null &&
                            !this.BackgroundWorkerEditMaintain.IsBusy
                         )
                       )
                    {
                        BackgroundWorkerEditMaintain = new BackgroundWorker();
                        BackgroundWorkerEditMaintain.WorkerSupportsCancellation = true;

                        BackgroundWorkerEditMaintain.DoWork += this.DoEditMaintainAccount;
                        BackgroundWorkerEditMaintain.RunWorkerCompleted += this.AfterEditMaintainAccount;

                        BackgroundWorkerEditMaintain.RunWorkerAsync();
                    }
                }
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterActivatePreReg( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return;

            AccountProxy account = this.Model as AccountProxy;
            var empiPatient = account.Activity.EmpiPatient;
            
            if ( e.Cancelled )
            {
                this.RefreshPatientAccountsList();
                // if this is a clickOnce button, re-enable

                if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                {
                    this.ClickedButton.Enabled = true;
                }
            }
            else if ( e.Error != null )
            {
                throw e.Error;
            }
            else
            {
                // success 
                if ( account == null ) return;

                // ensure that the account status has not changed since the user retrieved the account
                // 1. account is no longer PreReg
                // 2. account is cancelled
                // 3. account is locked

                if ( account.KindOfVisit.Code != VisitType.PREREG_PATIENT )
                {
                    //MessageBox.Show(UIErrorMessages.ACTIVATE_INVALID_NOT_PREREG, "Error",                         
                    MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_PREREG, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );
                    this.ReleaseAccountLockByCurrentUserFor(account);
                    this.RefreshPatientAccountsList();
                    return;
                }

                if ( account.IsCanceled )
                {
                    MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_CANCELED, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );
                    this.RefreshPatientAccountsList();
                    return;
                }

                //account.IsShortRegistered = true; //  temporary value - set to test Edit/Maintain of short reg accounts
                if ( account.IsShortRegistered )
                {
                    // Setting this property on AccountView will direct the application to the 
                    // 8-tab view for a Short-Registered account instead of the regular 12-tab view
                    AccountView.IsShortRegAccount = true;
                    account.Activity = new ShortRegistrationActivity();
                    CurrentActivity = new ShortRegistrationActivity();
                    account.Activity.EmpiPatient = empiPatient;
                    CurrentActivity.EmpiPatient = empiPatient;
                    account.Activity.AssociatedActivityType = typeof( ActivatePreRegistrationActivity );

                    if ( ParentForm != null )
                    {
                        ( ( PatientAccessView )ParentForm ).LoadShortRegistrationView();
                    }
                }
                else if ( account.IsNewBorn )
                {
                    AccountView.IsShortRegAccount = false;
                    account.Activity = new AdmitNewbornActivity();
                    CurrentActivity = new AdmitNewbornActivity();
                    account.Activity.EmpiPatient = empiPatient;
                    CurrentActivity.EmpiPatient = empiPatient;
                    account.Activity.AssociatedActivityType = typeof( ActivatePreRegistrationActivity );

                    if ( ParentForm != null )
                    {
                        ( (PatientAccessView)ParentForm ).LoadRegistrationView(CurrentActivity);
                    }
                }
                else
                {
                    AccountView.IsShortRegAccount = false;

                    account.Activity = new RegistrationActivity();
                    CurrentActivity = new RegistrationActivity();
                    account.Activity.EmpiPatient = empiPatient;
                    CurrentActivity.EmpiPatient = empiPatient;
                    account.Activity.AssociatedActivityType = typeof( ActivatePreRegistrationActivity );

                    if ( ParentForm != null )
                    {
                        ( (PatientAccessView)ParentForm ).LoadRegistrationView();
                    }
                }

                if ( Parent != null && Parent.GetType() == typeof( MasterPatientIndexView ) )
                {
                    ( ( MasterPatientIndexView )Parent ).RemoveSearch();
                }

                LooseArgs args = new LooseArgs( account );

                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                SearchEventAggregator.GetInstance().RaiseActivatePreregisteredAccountEvent( this, args );

                this.pnlProgress.Visible = false;
                this.pnlProgress.SendToBack();
            }
        }
        

        private void DoActivatePreReg( object sender, DoWorkEventArgs e )
        {
            if ( !AccountActivityService.PlaceLockOn( this.Model as AccountProxy, String.Empty ) )
            {
                e.Cancel = true;
                return;
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.BackgroundWorkerActivatePreReg.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }
        private IAccount GetAccountFrom(IAccount selectedAccount)
        {
            
            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            IAccount account = null;

            if (selectedAccount != null)
            {
                account = broker.AccountProxyFor(User.GetCurrent().Facility.Code,
                                                 selectedAccount.Patient.MedicalRecordNumber,
                                                 selectedAccount.AccountNumber);
            }
            return account;
        }
        
        private void ActivatePregAccount()
        {
            this.BeforeWork();
            //In order to test presenter, use prsenter's property--SelectedAccount instead of ListView.SelectedItems[0].Tag
            //IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
            IAccount account = this.GetAccountFrom(this.SelectedAccount);
            if ( account != null )
            {
                account.Activity = CurrentActivity;
                ((PatientAccessView)this.ParentForm).CurrentActivity.EmpiPatient = account.Activity.EmpiPatient;
                account.Patient.InterFacilityTransferAccount = this.SelectedAccount.Patient.InterFacilityTransferAccount;
                if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
                {
                    AccountActivityService.DisplayAccountLockedMsg();
                    this.RefreshPatientAccountsList();

                    // if this is a clickOnce button, re-enable

                    if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                    {
                        this.ClickedButton.Enabled = true;
                    }
                    return;
                }
                else if (account.IsCanceled || account.IsCanceledPreRegistration)
                {
                    AccountActivityService.DisplayAccountCanceledMsg();

                    this.RefreshPatientAccountsList();
                    return;
                }
                else
                {
                    if ( account.IsNewBorn )
                    {
                        account.Activity = new AdmitNewbornActivity();
                        account.Activity.AssociatedActivityType = typeof(ActivatePreRegistrationActivity);
                    }
                    else
                        account.Activity = ( (PatientAccessView)this.ParentForm ).CurrentActivity;
                    account.IsShortRegistered = false;
                    this.Model = account;

                    if ( this.BackgroundWorkerActivatePreReg == null
                         ||
                         ( this.BackgroundWorkerActivatePreReg != null
                           && !this.BackgroundWorkerActivatePreReg.IsBusy )
                        )
                    {
                        BackgroundWorkerActivatePreReg = new BackgroundWorker();
                        BackgroundWorkerActivatePreReg.WorkerSupportsCancellation = true;

                        BackgroundWorkerActivatePreReg.DoWork += this.DoActivatePreReg;
                        BackgroundWorkerActivatePreReg.RunWorkerCompleted += this.AfterActivatePreReg;

                        BackgroundWorkerActivatePreReg.RunWorkerAsync();
                    }
                }
            }
        }

        private void ConvertToShortPrereg()
        {
            this.lblNoVisits.Visible = false;
            this.pnlProgress.Visible = true;
            this.progressPanel1.Visible = true;
            this.pnlProgress.BringToFront();
            this.progressPanel1.BringToFront();
            this.progressPanel1.Refresh();

            Logger.Debug("In ConvertToShortPrereg...");

            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                account.IsShortRegistered = true;
                this.Model = account;

                if ( account != null )
                {
                    if ( this.BackgroundWorkerEditMaintain == null ||
                         (
                            this.BackgroundWorkerEditMaintain != null &&
                            !this.BackgroundWorkerEditMaintain.IsBusy
                         )
                       )
                    {
                        BackgroundWorkerEditMaintain = new BackgroundWorker();
                        BackgroundWorkerEditMaintain.WorkerSupportsCancellation = true;

                        BackgroundWorkerEditMaintain.DoWork += this.DoConvertToShortPrereg;
                        BackgroundWorkerEditMaintain.RunWorkerCompleted += this.AfterConvertToShortPrereg;
                        BackgroundWorkerEditMaintain.RunWorkerAsync();
                    }
                }
            }
        }

        private void ActivatePregAccountShort()
        {
            this.BeforeWork();

            IAccount account = this.GetAccountFrom(this.SelectedAccount);

            if (account != null)
            {
                account.Activity = CurrentActivity;
                ((PatientAccessView)this.ParentForm).CurrentActivity.EmpiPatient = account.Activity.EmpiPatient;
                account.Patient.InterFacilityTransferAccount = this.SelectedAccount.Patient.InterFacilityTransferAccount;
                if (AccountLockStatus.IsAccountLocked(account, User.GetCurrent()))
                {
                    AccountActivityService.DisplayAccountLockedMsg();
                    this.RefreshPatientAccountsList();

                    // if this is a clickOnce button, re-enable

                    if (this.ClickedButton.GetType() == typeof(ClickOnceLoggingButton))
                    {
                        this.ClickedButton.Enabled = true;
                    }
                    return;
                }
                else if (account.IsCanceled || account.IsCanceledPreRegistration)
                {
                    AccountActivityService.DisplayAccountCanceledMsg();

                    this.RefreshPatientAccountsList();
                    return;

                }
                else
                {
                    account.Activity = ((PatientAccessView)this.ParentForm).CurrentActivity;

                    account.IsShortRegistered = true;
                    this.Model = account;

                    if (this.BackgroundWorkerActivatePreReg == null
                         ||
                         (this.BackgroundWorkerActivatePreReg != null
                           && !this.BackgroundWorkerActivatePreReg.IsBusy)
                        )
                    {
                        BackgroundWorkerActivatePreReg = new BackgroundWorker();
                        BackgroundWorkerActivatePreReg.WorkerSupportsCancellation = true;

                        BackgroundWorkerActivatePreReg.DoWork += this.DoActivatePreReg;
                        BackgroundWorkerActivatePreReg.RunWorkerCompleted += this.AfterActivatePreReg;

                        BackgroundWorkerActivatePreReg.RunWorkerAsync();
                    }
                }
            }
        }
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterCompletePostMSE( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return;

            AccountProxy account = this.Model as AccountProxy;

            if ( e.Cancelled )
            {
                this.RefreshPatientAccountsList();
                // if this is a clickOnce button, re-enable

                if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                {
                    this.ClickedButton.Enabled = true;
                }
            }
            else if ( e.Error != null )
            {
                throw e.Error;
            }
            else
            {
                LooseArgs args = new LooseArgs( account );

                //Raise ActivityStart event
                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );

                this.pnlProgress.Visible = false;
                this.pnlProgress.SendToBack();
            }
        }

        private void DoCompletePostMSE( object sender, DoWorkEventArgs e )
        {
            if ( !AccountActivityService.PlaceLockOn( this.Model as AccountProxy, String.Empty ) )
            {
                e.Cancel = true;
                return;
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.BackgroundWorkerCompletePostMSE.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void CompletePostMseRegistration()
        {
            this.BeforeWork();

            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                if ( account != null )
                {
                    if ( account.KindOfVisit.Code != VisitType.EMERGENCY_PATIENT &&
                        ( account.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) == false ) )
                    {
                        MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_PRE_MSE, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1 );
                        // if this is a clickOnce button, re-enable

                        if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }

                    account.Activity = new PostMSERegistrationActivity();

                    if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
                    {
                        AccountActivityService.DisplayAccountLockedMsg();
                        this.RefreshPatientAccountsList();
                        // if this is a clickOnce button, re-enable

                        if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }
                    else
                    {
                        this.Model = account;

                        if ( this.BackgroundWorkerCompletePostMSE == null
                             ||
                             ( this.BackgroundWorkerCompletePostMSE != null
                               && !this.BackgroundWorkerCompletePostMSE.IsBusy )
                            )
                        {
                            BackgroundWorkerCompletePostMSE = new BackgroundWorker();
                            BackgroundWorkerCompletePostMSE.WorkerSupportsCancellation = true;

                            BackgroundWorkerCompletePostMSE.DoWork += this.DoCompletePostMSE;
                            BackgroundWorkerCompletePostMSE.RunWorkerCompleted += this.AfterCompletePostMSE;
                            BackgroundWorkerCompletePostMSE.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        private void CompleteUCCPostMseRegistration()
        {
            this.BeforeWork();

            if ((this.PatientAccounts.SelectedItems.Count > 0) && (this.PatientAccounts.SelectedItems[0] != null))
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                if (account != null)
                {
                    if (account.KindOfVisit.Code != VisitType.OUTPATIENT &&
                        (account.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE) == false))
                    {
                        MessageBox.Show(UIErrorMessages.PATIENT_ACCTS_NOT_UC_PRE_MSE, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                        // if this is a clickOnce button, re-enable

                        if (this.ClickedButton.GetType() == typeof(ClickOnceLoggingButton))
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }

                    account.Activity = new UCCPostMseRegistrationActivity();

                    if (AccountLockStatus.IsAccountLocked(account, User.GetCurrent()))
                    {
                        AccountActivityService.DisplayAccountLockedMsg();
                        this.RefreshPatientAccountsList();
                        // if this is a clickOnce button, re-enable

                        if (this.ClickedButton.GetType() == typeof(ClickOnceLoggingButton))
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }
                    else
                    {
                        this.Model = account;

                        if (this.BackgroundWorkerCompletePostMSE == null
                             ||
                             (this.BackgroundWorkerCompletePostMSE != null
                               && !this.BackgroundWorkerCompletePostMSE.IsBusy)
                            )
                        {
                            BackgroundWorkerCompletePostMSE = new BackgroundWorker();
                            BackgroundWorkerCompletePostMSE.WorkerSupportsCancellation = true;

                            BackgroundWorkerCompletePostMSE.DoWork += this.DoCompletePostMSE;
                            BackgroundWorkerCompletePostMSE.RunWorkerCompleted += this.AfterCompletePostMSE;
                            BackgroundWorkerCompletePostMSE.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        /// <exception cref="LoadAccountTimeoutException"><c>LoadAccountTimeoutException</c>.</exception>
        private void CreateNewbornAccount()
        {
            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                if ( account != null )
                {
                    if ( account.Patient.Sex.Code != Gender.FEMALE_CODE ||
                        account.KindOfVisit.Code != VisitType.INPATIENT ||
                        account.HospitalService.Code != "16" ||
                        account.DischargeDate != DateTime.MinValue )
                    {
                        MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_MATCH_CRITERIA_1 +
                                        UIErrorMessages.PATIENT_ACCTS_NOT_MATCH_CRITERIA_2, "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1 );
                        this.RefreshPatientAccountsList();
                        // if this is a clickOnce button, re-enable

                        if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }

                    if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
                    {
                        DialogResult result = MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_CREATING_FROM_LOCKED_ACCT_1 +
                                                               UIErrorMessages.PATIENT_ACCTS_CREATING_FROM_LOCKED_ACCT_2,
                                                               "Creating Account Based on Locked Account",
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                                               MessageBoxDefaultButton.Button1 );

                        if ( result == DialogResult.No )
                        {
                            // if this is a clickOnce button, re-enable

                            if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                            {
                                this.ClickedButton.Enabled = true;
                            }
                            return;
                        }
                    }
                    else
                    {
                        //Lock mother's account
                        account.Activity = new AdmitNewbornActivity();
                        if ( !account.Activity.ReadOnlyAccount() )
                        {
                            if ( !AccountActivityService.PlaceLockOn( account, String.Empty ) )
                            {
                                return;
                            }
                        }
                    }
                    IAccount newBornAccount = new Account();

                    try
                    {
                        account.Activity = CurrentActivity;
                        newBornAccount.Activity = CurrentActivity;
                        newBornAccount = CurrentActivity.CreateAccountForActivityFrom( account );
                    }
                    catch ( RemotingTimeoutException )
                    {
                        throw new LoadAccountTimeoutException();
                    }

                    if ( this.CurrentActivity.AssociatedActivityType == typeof( PreRegistrationWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof( RegistrationWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof( PreMSERegistrationWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof( AdmitNewbornWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof(ShortRegistrationWithOfflineActivity)
                        || this.CurrentActivity.AssociatedActivityType == typeof(ShortPreRegistrationWithOfflineActivity))
                    {
                        // launch popup to collect AccountNumber and/or MedicalRecordNumber

                        this.enterOfflineInfo = new EnterOfflineInfo();
                        this.enterOfflineInfo.Model_Patient = null;
                        this.enterOfflineInfo.Model_IAccount = newBornAccount;
                        this.enterOfflineInfo.UpdateView();
                        DialogResult result = this.enterOfflineInfo.ShowDialog( this );

                        if ( result == DialogResult.Cancel )
                        {
                            // if this is a clickOnce button, re-enable

                            if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                            {
                                this.ClickedButton.Enabled = true;
                            }
                            //release mother's account
                            this.ReleaseAccountLockByCurrentUserFor(account);
                            return;
                        }
                    }

                    newBornAccount.IsNew = true;
                    LooseArgs args = new LooseArgs( newBornAccount );

                    ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                    SearchEventAggregator.GetInstance().RaiseCreateNewBornAccountEvent( this, args );
                }
            }
        }

        /// <exception cref="LoadAccountTimeoutException"><c>LoadAccountTimeoutException</c>.</exception>
        private void CreatePreNewbornAccount()
        {
            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.PatientAccounts.SelectedItems[0].Tag as IAccount;
                if ( account != null )
                {
                    if ( (account.Patient.Sex.Code != Gender.FEMALE_CODE ||
                        account.KindOfVisit.Code != VisitType.INPATIENT ||
                        account.HospitalService.Code != "16" ||
                        account.DischargeDate != DateTime.MinValue ) &&
                            ( account.Patient.Sex.Code != Gender.FEMALE_CODE ||
                            account.KindOfVisit.Code != VisitType.PREREG_PATIENT ||
                            account.IsQuickRegistered ||
                            account.HospitalService.Code != "35" ||
                            account.IsCanceledPreRegistration || account.IsCanceled ||
                            account.AdmitDate.Date < User.GetCurrent().Facility.GetCurrentDateTime().Date  ||
                            account.AdmitDate.Date > User.GetCurrent().Facility.GetCurrentDateTime().Date.AddDays( 90 ) )
                        )
                    {
                        MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_NOT_MATCH_CRITERIA_1 +
                                        UIErrorMessages.PATIENT_ACCTS_NOT_MATCH_CRITERIA_2 + 
                                        UIErrorMessages.PATIENT_ACCTS_NOT_MATCH_CRITERIA_3, "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1 );
                        this.RefreshPatientAccountsList();
                        // if this is a clickOnce button, re-enable

                        if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }

                    if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
                    {
                        DialogResult result = MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_CREATING_FROM_LOCKED_ACCT_1 +
                                                               UIErrorMessages.PATIENT_ACCTS_CREATING_FROM_LOCKED_ACCT_2,
                                                               "Creating Account Based on Locked Account",
                                                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                                               MessageBoxDefaultButton.Button1 );

                        if ( result == DialogResult.No )
                        {
                            // if this is a clickOnce button, re-enable

                            if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                            {
                                this.ClickedButton.Enabled = true;
                            }
                            return;
                        }
                    }
                    else
                    {
                        //Lock mother's account
                        account.Activity = new PreAdmitNewbornActivity();
                        if ( !account.Activity.ReadOnlyAccount() )
                        {
                            if ( !AccountActivityService.PlaceLockOn( account, String.Empty ) )
                            {
                                return;
                            }
                        }
                    }
                    IAccount newBornAccount = new Account();

                    try
                    {
                        account.Activity = CurrentActivity;
                        newBornAccount.Activity = CurrentActivity;
                        newBornAccount = CurrentActivity.CreateAccountForActivityFrom( account );
                    }
                    catch ( RemotingTimeoutException )
                    {
                        throw new LoadAccountTimeoutException();
                    }

                    if ( this.CurrentActivity.AssociatedActivityType == typeof( PreRegistrationWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof( RegistrationWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof( PreMSERegistrationWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof( PreAdmitNewbornWithOfflineActivity )
                        || this.CurrentActivity.AssociatedActivityType == typeof(ShortPreRegistrationWithOfflineActivity)
                        || this.CurrentActivity.AssociatedActivityType == typeof(ShortRegistrationWithOfflineActivity)
                        )
                    {
                        // launch popup to collect AccountNumber and/or MedicalRecordNumber

                        this.enterOfflineInfo = new EnterOfflineInfo();
                        this.enterOfflineInfo.Model_Patient = null;
                        this.enterOfflineInfo.Model_IAccount = newBornAccount;
                        this.enterOfflineInfo.UpdateView();
                        DialogResult result = this.enterOfflineInfo.ShowDialog( this );

                        if ( result == DialogResult.Cancel )
                        {
                            // if this is a clickOnce button, re-enable

                            if ( this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                            {
                                this.ClickedButton.Enabled = true;
                            }
                            //release mother's account
                            this.ReleaseAccountLockByCurrentUserFor(account);
                            return;
                        }
                    }

                    newBornAccount.IsNew = true;
                    LooseArgs args = new LooseArgs( newBornAccount );

                    ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                    SearchEventAggregator.GetInstance().RaiseCreateNewBornAccountEvent( this, args );
                }
            }
        }
        private void ReleaseAccountLockByCurrentUserFor(IAccount account)
        {
            if (AccountLockStatus.IsAccountLocked(account, User.GetCurrent()))
            {
                AccountActivityService.ReleaseAccountlock(account);
            }
        }

        private void DoContinueActivity( object sender, DoWorkEventArgs e )
        {
            AccountProxy account = this.Model as AccountProxy;

            if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
            {
                AccountActivityService.DisplayAccountLockedMsg();
                e.Cancel = true;
                // if this is a clickOnce button, re-enable

                #region Dean Bortell Defect 34743 31 July 2007
                //in AfterEditMaintainAccount ln1306, the e.Cancel is 
                //checked and if it is true the following code is done anyway
                //this code causes the exception and is unnecessary so I commented
                //it.  Testing should remove it upon verification of functionality

                #endregion

                return;
            }
            else
            {
                if ( !AccountActivityService.PlaceLockOn( account, String.Empty ) )
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (account.IsCanceled || account.IsCanceledPreRegistration)
            {
                e.Cancel = true;
                return;
            }
            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.BackgroundWorkerContinueActivity.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterContinueActivity( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return;

            AccountProxy account = this.Model as AccountProxy;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
                this.RefreshPatientAccountsList();
                // if this is a clickOnce button, re-enable

                if ( this.ClickedButton != null && this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                {
                    this.ClickedButton.Enabled = true;
                }
            }
            else if ( e.Error != null )
            {
                throw e.Error;
            }
            else
            {
                if ( account.IsCanceled )
                {
                    MessageBox.Show( UIErrorMessages.PATIENT_ACCTS_CONTINUE_CANCEL, "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1 );
                    this.UpdateView();
                    return;
                }
                LooseArgs args = new LooseArgs( account );

                //Raise ActivityStart event
                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );

                this.pnlProgress.Visible = false;
                this.pnlProgress.SendToBack();
            }
        }

        private void ContinueActivity()
        {
            this.BeforeWork();

            if ( ( this.PatientAccounts.SelectedItems.Count > 0 ) && ( this.PatientAccounts.SelectedItems[0] != null ) )
            {
                IAccount account = this.GetAccountFrom(this.PatientAccounts.SelectedItems[0].Tag as IAccount);
                this.SelectedAccount = account;
                this.Model = account;

                if ( account != null )
                {
                    if ( AccountLockStatus.IsAccountLocked( account, User.GetCurrent() ) )
                    {
                        AccountActivityService.DisplayAccountLockedMsg();
                        this.RefreshPatientAccountsList();
                        // if this is a clickOnce button, re-enable

                        if ( this.ClickedButton != null && this.ClickedButton.GetType() == typeof( ClickOnceLoggingButton ) )
                        {
                            this.ClickedButton.Enabled = true;
                        }
                        return;
                    }
                    else if (account.IsCanceled || account.IsCanceledPreRegistration)
                    {
                        AccountActivityService.DisplayAccountCanceledMsg();
                        this.RefreshPatientAccountsList();
                        this.EnableButtonsForActivity();
                        return;
                    }
                    else
                    {
                        Activity currentActivity = ( (PatientAccessView)this.ParentForm ).CurrentActivity;
                        account.Activity = currentActivity;

                        if ( currentActivity != null && !currentActivity.ReadOnlyAccount() )
                        {
                            if ( this.BackgroundWorkerContinueActivity == null
                                    ||
                                    ( this.BackgroundWorkerContinueActivity != null
                                        && !this.BackgroundWorkerContinueActivity.IsBusy )
                                  )
                            {
                                BackgroundWorkerContinueActivity = new BackgroundWorker();
                                BackgroundWorkerContinueActivity.WorkerSupportsCancellation = true;

                                BackgroundWorkerContinueActivity.DoWork += this.DoContinueActivity;
                                BackgroundWorkerContinueActivity.RunWorkerCompleted += this.AfterContinueActivity;

                                BackgroundWorkerContinueActivity.RunWorkerAsync();
                            }
                        }
                    }
                }
            }
        }

        private void patientAccounts_GotFocus( object sender, EventArgs e )
        {
            CheckForSelectedItem();
        }

        private void CheckForSelectedItem()
        {
            bool rowSelected = false;

            int totalNumberOfRows = PatientAccounts.Items.Count;

            foreach ( ListViewItem item in PatientAccounts.Items )
            {
                if ( item.Selected )
                {
                    rowSelected = true;
                    break;
                }
            }

            if ( !rowSelected && totalNumberOfRows > 0 )
            {
                this.PatientAccounts.Items[0].Selected = true;
            }
        }

        private void DisplayActivity()
        {
            this.activityContextView1.Model = CurrentActivity;
            this.activityContextView1.UpdateView();
        }

        private void DisplayPatient()
        {
            AbstractPatient abstractPatient = this.Model as AbstractPatient;

            if ( abstractPatient != null )
            {
                this.patientContextView1.Model = abstractPatient;
                this.patientContextView1.UpdateView();
            }
        }

        private string GetCriteria()
        {
            string result = string.Empty;
            DateTime minDate = DateTime.MinValue;

            switch ( CurrentActivity.GetType().Name )
            {
                case "PostMSERegistrationActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.EMERGENCY_PATIENT + "' AND FinancialClass = '37'";
                    }
                    break;
                case "UCCPostMseRegistrationActivity":
                    if (IsFilterOn)
                    {
                        result = "PatientType = '" + VisitType.OUTPATIENT + "' AND FinancialClass = '37'";
                    }
                    break;
                case "CancelPreRegActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.PREREG_PATIENT + "' AND DerivedVisitType NOT = '" + Account.PRE_CAN + "'";
                    }
                    break;
                case "EditDischargeDataActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.INPATIENT + "' AND DischargeDate not = '" + minDate.ToString( "MM/dd/yyyy" ) + "'";
                    }
                    break;
                case "EditRecurringDischargeActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.RECURRING_PATIENT + "'";
                    }
                    break;
                case "DischargeActivity":
                case "PreDischargeActivity":
                    if ( IsFilterOn )
                    {
                        result = "(PatientType  = '" + VisitType.INPATIENT + "' OR PatientType  = '" + VisitType.OUTPATIENT +
                             "' OR PatientType  = '" + VisitType.NON_PATIENT +
                            "' OR ( PatientType  = '" + VisitType.EMERGENCY_PATIENT + "' AND HSV = '65'))" +
                            " AND FinancialClass NOT = '37'   AND DischargeDate = '" + minDate.ToString("MM/dd/yyyy") + "'";
                    }
                    break;
                case "CancelInpatientDischargeActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.INPATIENT + "' AND DischargeDate NOT = '" + minDate.ToString( "MM/dd/yyyy" ) + "'";
                    }
                    break;
                case "CancelOutpatientDischargeActivity":
                    if ( IsFilterOn )
                    {
                        result = "((( PatientType = '" + VisitType.OUTPATIENT + "' AND FinancialClass NOT = '37')" + " AND ( HSV = '57' OR HSV = '58' OR HSV = '59' OR HSV = 'FO' OR HSV = 'LD' OR HSV = 'LB' OR HSV = 'OH' )) OR ( PatientType = '" + VisitType.EMERGENCY_PATIENT + "' AND HSV = '65' )) AND DischargeDate NOT = '" + minDate.ToString("MM/dd/yyyy") + "'";
                    }
                    break;
                case "PreAdmitNewbornActivity":
                    if ( IsFilterOn )
                    {
                        //SR 1557 Mother can be either 1: PT=1, HSV=16, Female, not discharged
                        //                          or 2: PT=0, HSV=35, not canceledPrereg, Female, AdmitDate is within 90 days from today( not consider Admit Time), and not QAC Account             
                        result = "(PatientType = '" + VisitType.INPATIENT + "' AND HSV = '16' AND Gender = '" + Gender.FEMALE_CODE + "' AND DischargeDate = '" + minDate.ToString( "MM/dd/yyyy" ) + "') OR "
                                + "(PatientType = '" + VisitType.PREREG_PATIENT + "' AND HSV = '35' AND Gender = '" + Gender.FEMALE_CODE + "' AND DerivedVisitType NOT ='" +Account.PRE_CAN
                                        + "' AND NOT QacReg"
                                        + " AND AdmitDate >= '" + User.GetCurrent().Facility.GetCurrentDateTime().Date.ToString( "MM/dd/yyyy" )
                                        + "' AND AdmitDate <= '" + User.GetCurrent().Facility.GetCurrentDateTime().Date.AddDays( 90 ).ToString( "MM/dd/yyyy" ) + "')";
                    }
                    break;
                case "AdmitNewbornActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.INPATIENT + "' AND HSV = '16' AND Gender = '" + Gender.FEMALE_CODE + "' AND DischargeDate = '" + minDate.ToString( "MM/dd/yyyy" ) + "'";
                    }
                    break;
                case "TransferActivity":
                    if (IsFilterOn)
                    {
                        result = "( PatientType = '" + VisitType.INPATIENT + "' OR PatientType = '" + VisitType.RECURRING_PATIENT + "' OR PatientType = '" + VisitType.NON_PATIENT + "' ) OR" +
                                 " ( PatientType = '" + VisitType.EMERGENCY_PATIENT + "' AND FinancialClass NOT = '37' ) OR " +
                                 "( PatientType = '" + VisitType.OUTPATIENT + "' AND FinancialClass NOT = '37' ) AND DischargeDate = '" + minDate.ToString("MM/dd/yyyy") + "'";
                    }
                    break;
                case "TransferInToOutActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.INPATIENT + "' AND DischargeDate = '" + minDate.ToString( "MM/dd/yyyy" ) + "'";
                    }
                    break;
                case "TransferOutToInActivity":
                    if ( IsFilterOn )
                    {
                        result = "(PatientType = '" + VisitType.OUTPATIENT + "' AND FinancialClass NOT = '37')" + " OR ( PatientType = '" + VisitType.EMERGENCY_PATIENT + "' AND FinancialClass NOT = '37' )";
                    }
                    break;
                case "CancelInpatientStatusActivity":
                    if ( IsFilterOn )
                    {
                        result = "PatientType = '" + VisitType.INPATIENT + "' AND AdmitDate = '" +
                                User.GetCurrent().Facility.GetCurrentDateTime().Date.ToString( "MM/dd/yyyy" ) + "' " +
                                "AND DischargeDate = '" + minDate.ToString( "MM/dd/yyyy" ) + "'";
                    }
                    break;
                case Activity.TransferOutPatientToErPatient:
                    if (IsFilterOn)
                    {
                        result = "( ( PatientType = '" + VisitType.OUTPATIENT + "' AND FinancialClass NOT = '37')" + " AND ( HSV = '57' OR HSV = '58' OR HSV = '59' OR HSV = 'FO' OR HSV = 'LD' OR HSV = 'LB' OR HSV = 'OH')) ";
                    }
                    break;
                case Activity.TransferErPatientToOutPatient:
                    if (IsFilterOn)
                    {
                        result = "PatientType = '" + VisitType.EMERGENCY_PATIENT   + "'";
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private void BindResults()
        {
            foreach ( AccountProxy account in filteredResults )
            {
                string time = CommonFormatting.MaskedTimeFormat( account.AdmitDate );
                string hour = time.Substring( 0, 2 );
                string minute = time.Substring( 2, 2 );

                ListViewItem item = new ListViewItem();

                item.Text = string.Empty;
                if ( account.AccountLock.IsLocked )
                {
                    item.ImageIndex = 0;

                }

                item.Tag = account;
                //AbstractPatient abstractPatient = this.Model as AbstractPatient;
                //var IFXFRAccount= GetInterFacilityTransferAccounts(abstractPatient.MedicalRecordNumber, abstractPatient.Facility.Code, account.AccountNumber);
                
                //if (IFXFRAccount != null)
                //{
                //    if (IFXFRAccount.ToAccountNumber == account.AccountNumber)
                //    {
                //        item.SubItems.Add("I");
                //    }
                //    else
                //    {
                //        item.SubItems.Add("");
                //    }
                //}
                //else
                //{
                //    item.SubItems.Add("");
                //}
                

                item.SubItems.Add( account.AdmitDate.ToString( "MM/dd/yyyy " ) + hour + ":" + minute );

                // Constructor initializes the Account.DischargeDate to DateTime.MinValue by default.
                // Prevent the display of this value when no actual discharge date exists.
                if ( account.DischargeDate != DateTime.MinValue )
                {
                    item.SubItems.Add( account.DischargeDate.ToString( "MM/dd/yyyy" ) );
                }
                else
                {
                    item.SubItems.Add( String.Empty );
                }
                if (CurrentActivity.IsValidRegistrationActivityForDischargeDisposition)
                {
                    if (account.DischargeDisposition != null)
                    {
                        item.SubItems.Add(account.DischargeDisposition.ToCodedString());
                    }
                    else
                    {
                        item.SubItems.Add(String.Empty);
                    }
                }
                item.SubItems.Add(account.KindOfVisit.ToCodedString());
                item.SubItems.Add( account.DerivedVisitType );

                if ( account.HospitalService != null )
                {
                    item.SubItems.Add( account.HospitalService.ToCodedString() );
                }
                else
                {
                    item.SubItems.Add( String.Empty );
                }

                item.SubItems.Add( account.HospitalClinic.ToCodedString() );
                item.SubItems.Add( account.FinancialClass.ToCodedString() );
                item.SubItems.Add( account.AccountNumber.ToString() );

                if ( account.IsLocked )
                {
                    item.SubItems.Add( "1" );
                }
                else
                {
                    item.SubItems.Add( "0" );
                }
                item.SubItems.Add( string.Empty );

                PatientAccounts.Items.Add( item );
            }
        }

        private InterFacilityTransferAccount GetInterFacilityTransferAccounts(long MedicalRecordNumber,string selectedfacility,long ToAccountNumber)
        {
            i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility FromFacility = i_FacilityBroker.FacilityWith(selectedfacility.Trim());
            interFacilityTransferAccount = interfacilityTransferBroker.AccountsfromtransferlogforRegistration(MedicalRecordNumber, FromFacility, ToAccountNumber);
            return interFacilityTransferAccount;
        }

        private void HandleInterfacilityAcccountActivatePopup()
        {
            if (SelectedAccount.Facility.IsSATXEnabled)
            {
                //Popup warning Interfacility message to get From Hospital and From Account
                var anAccount = SelectedAccount;
                bool IsITFREnabled = interFacilityTransferFeatureManager.IsITFREnabled(User.GetCurrent().Facility, this.CurrentActivity);
                if (IsITFREnabled && anAccount.AccountNumber != 0 && anAccount.KindOfVisit.IsPreRegistrationPatient)
                {
                    interfacilityPopup = new InterfacilityPopup();
                    i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                    interFacilityTransferAccount = interfacilityTransferBroker.AccountsfromtransferlogforRegistration(anAccount.Patient.MedicalRecordNumber, anAccount.Facility, anAccount.AccountNumber);
                    interfacilityPopup.Title = "Interfacility Transfer";
                    if (interFacilityTransferAccount != null)
                    {
                        if (interFacilityTransferAccount.FromAccountNumber != 0)
                        {
                            Facility facility = i_FacilityBroker.FacilityWith(interFacilityTransferAccount.FromFacilityOid);
                            var dtFromAccount = interfacilityTransferBroker.GetAllAccountsForPatient(anAccount.Patient.MedicalRecordNumber, interFacilityTransferAccount.FromFacility);
                           
                            if (interFacilityTransferAccount.FromDischargeDate == DateTime.MinValue)
                            {
                                interfacilityPopup.HeaderText = " The selected patient has an active Account# - " + interFacilityTransferAccount.FromAccountNumber + ", Patient Type - "+ dtFromAccount[0].KindOfVisit.DisplayString + ", HSV - "+ dtFromAccount[0].HospitalService.DisplayString+" at the Hospital - " +
                                   facility.Description + ". Please discharge the patient first to proceed with the activation.";
                                interfacilityPopup.SetOKButton = true;
                                interfacilityPopup.ShowDialog(this);
                            }
                            else
                            {
                                interfacilityPopup.HeaderText = " The selected account has a transfer linked with the Hospital - " + facility.Description + " , Account# - " + interFacilityTransferAccount.FromAccountNumber + " , Patient Type - " + dtFromAccount[0].KindOfVisit.DisplayString + ", HSV - " + dtFromAccount[0].HospitalService.DisplayString + " . Do you want to activate this account?";
                                interfacilityPopup.SetOKButton = false;
                                interfacilityPopup.CancelActivity = true;
                                interfacilityPopup.ShowDialog(this);
                                if (!interfacilityPopup.CancelActivity)
                                {
                                    this.SelectedAccount.Patient.InterFacilityTransferAccount = interFacilityTransferAccount;
                                    this.ActivatePregAccount();
                                }
                            }
                        }
                    }
                    else
                    {
                        // interfacilityWarningPopup
                        this.ActivatePregAccount();
                    }
                }
                else
                {
                    this.ActivatePregAccount();
                }
            }
            else
            {
                this.ActivatePregAccount();
            }
        }

        private void HandleInterfacilityAcccountActivateShortPopup()
        {
            if (SelectedAccount.Facility.IsSATXEnabled)
            {
                //Popup warning Interfacility message to get From Hospital and From Account
                var anAccount = SelectedAccount;
                bool IsITFREnabled = interFacilityTransferFeatureManager.IsITFREnabled(User.GetCurrent().Facility, this.CurrentActivity);
                if (IsITFREnabled && anAccount.AccountNumber != 0 && anAccount.KindOfVisit.IsPreRegistrationPatient)
                {
                    interfacilityPopup = new InterfacilityPopup();
                    i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                    interFacilityTransferAccount = interfacilityTransferBroker.AccountsfromtransferlogforRegistration(anAccount.Patient.MedicalRecordNumber, anAccount.Facility, anAccount.AccountNumber);
                    interfacilityPopup.Title = "Interfacility Transfer";
                    if (interFacilityTransferAccount != null)
                    {
                        if (interFacilityTransferAccount.FromAccountNumber != 0)
                        {
                            Facility facility = i_FacilityBroker.FacilityWith(interFacilityTransferAccount.FromFacilityOid);
                            var dtFromAccount = interfacilityTransferBroker.GetAllAccountsForPatient(anAccount.Patient.MedicalRecordNumber, interFacilityTransferAccount.FromFacility);

                            if (interFacilityTransferAccount.FromDischargeDate == DateTime.MinValue)
                            {
                                interfacilityPopup.HeaderText = " The selected patient has an active Account# - " + interFacilityTransferAccount.FromAccountNumber + ", Patient Type - " + dtFromAccount[0].KindOfVisit.DisplayString + ", HSV - " + dtFromAccount[0].HospitalService.DisplayString + " at the Hospital - " +
                                   facility.Description + ". Please contact the sending facility to discharge the patient first and proceed with the activation.";
                                interfacilityPopup.SetOKButton = true;
                                interfacilityPopup.ShowDialog(this);
                            }
                            else
                            {
                                interfacilityPopup.HeaderText = " The selected account has a transfer linked with the Hospital - " + facility.Description + " , Account# - " + interFacilityTransferAccount.FromAccountNumber + " , Patient Type - " + dtFromAccount[0].KindOfVisit.DisplayString + ", HSV - " + dtFromAccount[0].HospitalService.DisplayString + " . Do you want to activate this account?";
                                interfacilityPopup.SetOKButton = false;
                                interfacilityPopup.CancelActivity = true;
                                interfacilityPopup.ShowDialog(this);
                                if (!interfacilityPopup.CancelActivity)
                                {
                                    this.SelectedAccount.Patient.InterFacilityTransferAccount = interFacilityTransferAccount;
                                    this.ActivatePregAccountShort();
                                }
                            }
                        }
                    }
                    else
                    {
                        // interfacilityWarningPopup
                        this.ActivatePregAccountShort();
                    }
                }
                else
                {
                    this.ActivatePregAccountShort();
                }
            }
            else
            {
                this.ActivatePregAccountShort();
            }
        }
        private ArrayList DisplayAccountsForInterfacilityTransfer()
        {
            ArrayList filteredResultsForERToInpatient = new ArrayList();
            var ITFRCriteria = "PatientType = '" + VisitType.EMERGENCY_PATIENT + "' AND FinancialClass NOT = '37'";
            if (filter != null)
            {
                filteredResultsForERToInpatient = filter.ExecuteFilter(ITFRCriteria);
            }



            return filteredResultsForERToInpatient;
        }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( PatientsAccountsView ) );
            this.Cbm = new CommandButtonManager();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.PatientAccounts = new System.Windows.Forms.ListView();
            this.chIfxfr = new System.Windows.Forms.ColumnHeader();
            this.chBmp = new System.Windows.Forms.ColumnHeader();
            this.chAdmitDate = new System.Windows.Forms.ColumnHeader();
            this.chDishDate = new System.Windows.Forms.ColumnHeader();
            this.chDishargeDisposition = new System.Windows.Forms.ColumnHeader();
            this.chPatientType = new System.Windows.Forms.ColumnHeader();
            this.chVisitType = new System.Windows.Forms.ColumnHeader();
            this.chHospitalService = new System.Windows.Forms.ColumnHeader();
            this.chClinic = new System.Windows.Forms.ColumnHeader();
            this.chFinancialClass = new System.Windows.Forms.ColumnHeader();
            this.chAccount = new System.Windows.Forms.ColumnHeader();
            this.imlLock = new System.Windows.Forms.ImageList( this.components );
            this.ttUserName = new System.Windows.Forms.ToolTip( this.components );
            this.lblNoVisits = new System.Windows.Forms.Label();
            this.patientContextView1 = new PatientAccess.UI.PatientContextView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnRefresh = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnReturnToSearch = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.activityContextView1 = new PatientAccess.UI.ActivityContextView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 1, 1 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 1003, 467 );
            this.progressPanel1.TabIndex = 0;
            // 
            // patientAccounts
            // 
            this.PatientAccounts.AutoArrange = false;
            this.PatientAccounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PatientAccounts.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] { 
            //this.chIfxfr,
            this.chBmp,
            this.chAdmitDate,
            this.chDishDate,
            this.chDishargeDisposition,
            this.chPatientType,
            this.chVisitType,
            this.chHospitalService,
            this.chClinic,
            this.chFinancialClass,
            this.chAccount
            
            } );
            this.PatientAccounts.FullRowSelect = true;
            this.PatientAccounts.GridLines = true;
            this.PatientAccounts.HideSelection = false;
            this.PatientAccounts.Location = new System.Drawing.Point( 0, 0 );
            this.PatientAccounts.MultiSelect = false;
            this.PatientAccounts.Name = "PatientAccounts";
            this.PatientAccounts.Size = new System.Drawing.Size( 1004, 464 );
            this.PatientAccounts.SmallImageList = this.imlLock;
            this.PatientAccounts.TabIndex = 2;
            this.PatientAccounts.UseCompatibleStateImageBehavior = false;
            this.PatientAccounts.View = System.Windows.Forms.View.Details;
            this.PatientAccounts.DoubleClick += new System.EventHandler( this.patientAccounts_DoubleClick );
            this.PatientAccounts.SelectedIndexChanged += new System.EventHandler( this.patientAccounts_SelectedIndexChanged );
            this.PatientAccounts.GotFocus += new System.EventHandler( this.patientAccounts_GotFocus );
            // 
            // chBmp
            // 
            this.chBmp.Text = "";
            this.chBmp.Width = 20;
            // 
            // chAdmitDate
            // 
            this.chAdmitDate.Text = "Admit Date/Time";
            this.chAdmitDate.Width = 120;
            // 
            // chDishDate
            // 
            this.chDishDate.Text = "Disch Date";
            this.chDishDate.Width = 90;
            // 
            // chDishargeDisposition
            // 
            this.chDishargeDisposition.Text = "Discharge Disposition";
            this.chDishargeDisposition.Width = 100;
            // 
            // chPatientType
            // 
            this.chPatientType.Text = "Patient Type";
            this.chPatientType.Width = 100;
            // 
            // chVisitType
            // 
            this.chVisitType.Text = "Visit Type";
            this.chVisitType.Width = 85;
            // 
            // chHospitalService
            // 
            this.chHospitalService.Text = "Hospital Service";
            this.chHospitalService.Width = 160;
            // 
            // chClinic
            // 
            this.chClinic.Text = "Clinic 1";
            this.chClinic.Width = 160;
            // 
            // chFinancialClass
            // 
            this.chFinancialClass.Text = "Financial Class";
            this.chFinancialClass.Width = 165;
            // 
            // chIfxfr
            // 
            this.chIfxfr.Text = "IFXFR";
            this.chIfxfr.Width = 50;
            // 
            // chAccount
            // 
            this.chAccount.Text = "Account";
            this.chAccount.Width = 80;
            // 
            // imlLock
            // 
            this.imlLock.ImageStream = ( (System.Windows.Forms.ImageListStreamer)( resources.GetObject( "imlLock.ImageStream" ) ) );
            this.imlLock.TransparentColor = System.Drawing.Color.Transparent;
            this.imlLock.Images.SetKeyName( 0, "lock_icon.gif" );
            // 
            // lblNoVisits
            // 
            this.lblNoVisits.BackColor = System.Drawing.Color.White;
            this.lblNoVisits.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoVisits.Location = new System.Drawing.Point( 0, 0 );
            this.lblNoVisits.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.lblNoVisits.Name = "lblNoVisits";
            this.lblNoVisits.Size = new System.Drawing.Size( 1004, 470 );
            this.lblNoVisits.TabIndex = 0;
            // 
            // patientContextView1
            // 
            this.patientContextView1.Account = null;
            this.patientContextView1.BackColor = System.Drawing.Color.White;
            this.patientContextView1.DateOfBirthText = "";
            this.patientContextView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView1.GenderLabelText = "";
            this.patientContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.patientContextView1.Model = null;
            this.patientContextView1.Name = "patientContextView1";
            this.patientContextView1.PatientNameText = "";
            this.patientContextView1.Size = new System.Drawing.Size( 1002, 47 );
            this.patientContextView1.SocialSecurityNumber = "";
            this.patientContextView1.TabIndex = 0;
            this.patientContextView1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.panel1.Controls.Add( this.PatientAccounts );
            this.panel1.Controls.Add( this.panel2 );
            this.panel1.Controls.Add( this.lblNoVisits );
            this.panel1.Location = new System.Drawing.Point( 9, 115 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 1004, 508 );
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add( this.panel4 );
            this.panel2.Controls.Add( this.panelButtons );
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point( 0, 476 );
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size( 1004, 33 );
            this.panel2.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.Controls.Add( this.btnRefresh );
            this.panel4.Controls.Add( this.btnReturnToSearch );
            this.panel4.Location = new System.Drawing.Point( -1, 1 );
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size( 201, 30 );
            this.panel4.TabIndex = 3;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.btnRefresh.Location = new System.Drawing.Point( 115, 6 );
            this.btnRefresh.Message = null;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size( 75, 23 );
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Re&fresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler( this.btnRefresh_Click );
            // 
            // btnReturnToSearch
            // 
            this.btnReturnToSearch.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.btnReturnToSearch.BackColor = System.Drawing.SystemColors.Control;
            this.btnReturnToSearch.Location = new System.Drawing.Point( 0, 6 );
            this.btnReturnToSearch.Message = null;
            this.btnReturnToSearch.Name = "btnReturnToSearch";
            this.btnReturnToSearch.Size = new System.Drawing.Size( 108, 23 );
            this.btnReturnToSearch.TabIndex = 3;
            this.btnReturnToSearch.Text = "< &Back to Search";
            this.btnReturnToSearch.UseVisualStyleBackColor = false;
            this.btnReturnToSearch.Click += new System.EventHandler( this.btnReturnToSearch_Click );
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.panelButtons.Location = new System.Drawing.Point( 235, 2 );
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size( 763, 33 );
            this.panelButtons.TabIndex = 5;
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 4 ) ) ) ), ( (int)( ( (byte)( 137 ) ) ) ), ( (int)( ( (byte)( 185 ) ) ) ) );
            this.contextLabel.Description = " Recent Account History";
            this.contextLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.contextLabel.Location = new System.Drawing.Point( 0, 0 );
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size( 1024, 23 );
            this.contextLabel.TabIndex = 0;
            this.contextLabel.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add( this.patientContextView1 );
            this.panel3.Location = new System.Drawing.Point( 9, 60 );
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size( 1004, 49 );
            this.panel3.TabIndex = 0;
            // 
            // activityContextView1
            // 
            this.activityContextView1.BackColor = System.Drawing.Color.White;
            this.activityContextView1.CurrentActivity = null;
            this.activityContextView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activityContextView1.Location = new System.Drawing.Point( 0, 0 );
            this.activityContextView1.Model = null;
            this.activityContextView1.Name = "activityContextView1";
            this.activityContextView1.Size = new System.Drawing.Size( 1002, 23 );
            this.activityContextView1.TabIndex = 1;
            this.activityContextView1.FilterOff += new System.EventHandler( this.activityContextView1_FilterOff );
            this.activityContextView1.FilterOn += new System.EventHandler( this.activityContextView1_FilterOn );
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add( this.activityContextView1 );
            this.panel5.Location = new System.Drawing.Point( 9, 31 );
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size( 1004, 25 );
            this.panel5.TabIndex = 1;
            // 
            // pnlProgress
            // 
            this.pnlProgress.BackColor = System.Drawing.Color.Black;
            this.pnlProgress.Controls.Add( this.progressPanel1 );
            this.pnlProgress.Location = new System.Drawing.Point( 8, 115 );
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size( 1005, 469 );
            this.pnlProgress.TabIndex = 0;
            // 
            // PatientsAccountsView
            // 
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.contextLabel );
            this.Controls.Add( this.panel5 );
            this.Controls.Add( this.panel3 );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.pnlProgress );
            this.Name = "PatientsAccountsView";
            this.Size = new System.Drawing.Size( 1024, 629 );
            this.Load += new System.EventHandler( this.PatientsAccountsView_Load );
            this.panel1.ResumeLayout( false );
            this.panel2.ResumeLayout( false );
            this.panel4.ResumeLayout( false );
            this.panel3.ResumeLayout( false );
            this.panel5.ResumeLayout( false );
            this.pnlProgress.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private Panel pnlProgress;

        #endregion

        #region Private Properties

        private LoggingButton ClickedButton { get; set; }

        #endregion

        #region Properties

        private ControlView ParentControl { get; set; }

        public Activity CurrentActivity { get; private set; }

        #endregion

        #region Construction and Finalization

        public PatientsAccountsView( ControlView parent )
        {            
            this.ParentControl = parent;

            this.CurrentActivity = ( (MasterPatientIndexView)this.ParentControl ).CurrentActivity;

            InitializeComponent();
            EnableThemesOn( this );
            this.patientContextView1.TabStop = false;

            if ( Cbm != null )
            {
                Cbm.LoadButtons( this.CurrentActivity );
            }

            this.Presenter = new PatientAccountsViewPresenter(this, User.GetCurrent().Facility, new MessageBoxAdapter(),this.CurrentActivity);
        }

        private PatientAccountsViewPresenter Presenter { get; set; }

        public IAccount SelectedAccount { get; set; }

        public ListView PatientAccounts { get; set; }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
                // need to ensure all 7 background worker threads are cancelled here (at 
                // the very least). So added a call to a new private method with 
                // appropriate code...
                PatientAccountsViewPresenter.CancelAllBackgroundWorkerThreads( this );
            }
            base.Dispose( disposing );
        }


        #endregion

        #region Data Elements

        public Account NewAccount { get; set; }

        public IAccount NewIAccount { get; set; }

        private ProgressPanel progressPanel1;

        public BackgroundWorker BackgroundWorkerRefresh { get; private set; }

        public BackgroundWorker BackgroundWorkerUpdate { get; private set; }

        public BackgroundWorker BackgroundWorkerEditMaintain { get; private set; }

        public BackgroundWorker BackgroundWorkerContinueActivity { get; private set; }

        public BackgroundWorker BackgroundWorkerActivatePreReg { get; private set; }

        public BackgroundWorker BackgroundWorkerCompletePostMSE { get; private set; }

        private Patient patient;
        
        public CommandButtonManager Cbm { get; private set; }

        private UserContextView contextLabel;
        private ActivityContextView activityContextView1;
        private AccountFilter filter;
        private PatientContextView patientContextView1;
        private EnterOfflineInfo enterOfflineInfo;

        private ColumnHeader chBmp;
        private ColumnHeader chAdmitDate;
        private ColumnHeader chDishDate;
        private ColumnHeader chDishargeDisposition;
        private ColumnHeader chPatientType;
        private ColumnHeader chHospitalService;
        private ColumnHeader chFinancialClass;
        private ColumnHeader chAccount;
        private ColumnHeader chVisitType;
        private ColumnHeader chClinic;
        private ColumnHeader chIfxfr;

        private ImageList imlLock;

        private ToolTip ttUserName;

        private Label lblNoVisits;

        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panelButtons;

        private LoggingButton btnReturnToSearch;
        private LoggingButton btnRefresh;

        private IContainer components;
        InterFacilityTransferAccount interFacilityTransferAccount = new InterFacilityTransferAccount();
        InterFacilityTransferFeatureManager interFacilityTransferFeatureManager = new InterFacilityTransferFeatureManager();
        private IInterfacilityTransferBroker interfacilityTransferBroker =
            BrokerFactory.BrokerOfType<IInterfacilityTransferBroker>();
        private bool IsFilterOn { get; set; }
        private string i_SearchedAccountNumber = String.Empty;
        ArrayList filteredResults = new ArrayList();

        private static readonly ILog Logger = LogManager.GetLogger( typeof( PatientsAccountsView ) );
        //InterfacilityWarningPopup interfacilityWarningPopup;
        InterfacilityPopup interfacilityPopup;
        IFacilityBroker i_FacilityBroker;
        #endregion

        #region Constants
        private const string ACCOUNT_CANCELED_MSG = "The requested action cannot proceed because the account has been canceled.";
        #endregion

    }

}
