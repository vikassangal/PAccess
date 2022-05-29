using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.SecurityService.Domain;
using Extensions.UI.Builder;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PreRegistrationViews;
using log4net;
using PatientAccess.Domain.UCCRegistration;
using Facility = Peradigm.Framework.Domain.Parties.Facility;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// Summary description for WorklistsView.
    /// </summary>
    public class WorklistsView : ControlView
    {
        #region Event Handlers

        private AccountProxy GetLatestAccountProxyForSelectedAccount()
        {
            if ( worklist.ToString().ToUpper().Equals("NO SHOW") )
            {
                return noShowWorklistView.GetLatestAccountProxyForSelectedAccount();
            }
            else if (worklist.ToString().ToUpper().Equals("PREREGISTRATION"))
            {
                return preRegWorklistView.GetLatestAccountProxyForSelectedAccount();
            }
            else
            {
                return null;
            }
        }

        private void btnActivateAccount_Click( object sender, EventArgs e )
        {
            Cursor = Cursors.WaitCursor;

            SelectedAccountProxy = GetLatestAccountProxyForSelectedAccount();

            if ( SelectedAccountProxy == null )
            {
                Cursor = Cursors.Default;
                btnActivateAccount.Enabled = true;
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

                RefreshCurrentWorklist();
                Cursor = Cursors.Default;
                return;
            }

            if ( AccountLockStatus.IsAccountLocked( SelectedAccountProxy, User.GetCurrent() ) )
            {
                Cursor = Cursors.Default;

                MessageBox.Show( UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );

                RefreshCurrentWorklist();
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
                        RefreshCurrentWorklist();
                        Cursor = Cursors.Default;
                        btnActivateAccount.Enabled = true;
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
                    account.Activity = new RegistrationActivity
                                           {AssociatedActivityType = typeof (ActivatePreRegistrationActivity)};
                    currentActivity = new RegistrationActivity();
                }

                if (ParentForm != null)
                {
                    ((PatientAccessView) ParentForm).LoadRegistrationView(currentActivity);
                }

                // Verify the account has not already been canceled
                if ( account.IsCanceled )
                {
                    MessageBox.Show( ACCOUNT_CANCELED_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1 );

                    RefreshCurrentWorklist();
                }
                else
                {
                    // raise the ActivatePreregisteredAccountEvent
                    SearchEventAggregator aggregator = SearchEventAggregator.GetInstance();

                    aggregator.RaiseActivatePreregisteredAccountEvent( this, new LooseArgs( SelectedAccountProxy ) );
                    Dispose();
                }
            }
            btnActivateAccount.Enabled = true;
            Cursor = Cursors.Default;
        }

         private void btnActivateShortAccount_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            SelectedAccountProxy =  GetLatestAccountProxyForSelectedAccount();

            if (SelectedAccountProxy == null)
            {
                Cursor = Cursors.Default;
                btnActivateShortAccount.Enabled = true;
                return;
            }

            // ensure that the account status has not changed since the user retrieved the account
            // 1. if account is no longer PreReg
            // 2. if account is locked


            if (SelectedAccountProxy.KindOfVisit.Code != VisitType.PREREG_PATIENT)
            {
                MessageBox.Show(UIErrorMessages.PATIENT_ACCTS_NOT_PREREG, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);

                RefreshCurrentWorklist();
                Cursor = Cursors.Default;
                return;
            }

            if (AccountLockStatus.IsAccountLocked(SelectedAccountProxy, User.GetCurrent()))
            {
                Cursor = Cursors.Default;

                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);

                RefreshCurrentWorklist();
            }

            else
            {
                SelectedAccountProxy.Activity = new ShortMaintenanceActivity { AssociatedActivityType = typeof(ShortRegistrationActivity) };

                // Pro-actively lock the account proxy
                if (!SelectedAccountProxy.Activity.ReadOnlyAccount())
                {
                    bool blnLocked = AccountActivityService.PlaceLockOn(SelectedAccountProxy, String.Empty);

                    if (!blnLocked)
                    {
                        RefreshCurrentWorklist();
                        Cursor = Cursors.Default;
                        btnActivateShortAccount.Enabled = true;
                        return;
                    }
                }
                //***************************************************************************************

                IAccount account = SelectedAccountProxy;
                account.IsShortRegistered = true;
                Cursor = Cursors.Default;

                
                AccountView.IsShortRegAccount = true;
                account.Activity = new ShortRegistrationActivity { AssociatedActivityType = typeof(ActivatePreRegistrationActivity) };
               
                if (ParentForm != null)
                {
                    ((PatientAccessView)ParentForm).LoadShortRegistrationView();
                }

                // Verify the account has not already been canceled
                if (account.IsCanceled)
                {
                    MessageBox.Show(ACCOUNT_CANCELED_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1);

                    RefreshCurrentWorklist();
                }
                else
                {
                    // raise the ActivatePreregisteredAccountEvent
                    SearchEventAggregator aggregator = SearchEventAggregator.GetInstance();

                    aggregator.RaiseActivatePreregisteredAccountEvent(this, new LooseArgs(SelectedAccountProxy));
                    Dispose();
                }
            }
            btnActivateShortAccount.Enabled = true;
            Cursor = Cursors.Default;
         }

        private void WorklistsView_Load( object sender, EventArgs e )
        {
            if ( listView.Items.Count > 0 )
            {
                if ( actionItemsToDisplay )
                {
                    listView.Items[0].Selected = true;
                    Invalidate();   // Force redraw when AccountView is closed
                }
            }
        }

        private void tabAccounts_SelectedIndexChanged( object sender, EventArgs e )
        {
            Cursor = Cursors.WaitCursor;
            UpdateView();
            Cursor = Cursors.Default;
        }

        private void ResetButtonClickEvent( object sender, EventArgs e )
        {
            ResetControls();
        }

        private void btnCancelRegistration_Click( object sender, EventArgs e )
        {
            Cursor = Cursors.WaitCursor;

            AccountProxy proxy =  GetLatestAccountProxyForSelectedAccount();

            if ( proxy == null )
            {
                Cursor = Cursors.Default;
                btnCancelRegistration.Enabled = true;
                return;
            }

            if ( AccountLockStatus.IsAccountLocked( proxy, User.GetCurrent() ) )
            {
                Cursor = Cursors.Default;

                MessageBox.Show( UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1 );

                RefreshCurrentWorklist();
            }

            else
            {
                //***************************************************************************************

                proxy.Activity = new CancelPreRegActivity();

                // Pro-actively lock the account proxy
                if ( !proxy.Activity.ReadOnlyAccount() )
                {
                    bool blnLocked = AccountActivityService.PlaceLockOn( proxy, String.Empty );

                    if ( !blnLocked )
                    {
                        RefreshCurrentWorklist();
                        Cursor = Cursors.Default;
                        btnCancelRegistration.Enabled = true;
                        return;
                    }
                }
                //***************************************************************************************

                AccountProxy account = proxy;

                Cursor = Cursors.Default;
                // Verify the account has not already been canceled

                if ( account.DerivedVisitType == Account.PRE_CAN )
                {
                    MessageBox.Show( ACCOUNT_CANCELED_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1 );

                    RefreshCurrentWorklist();
                }
                else
                {
                    foreach ( Control control in Controls )
                    {
                        if ( control != null )
                        {
                            try
                            {
                                control.Dispose();
                            }
                            catch ( Exception ex )
                            {
                                c_log.Error( "Failed to dispose of a control; " + ex.Message, ex );
                            }
                        }
                    }
                    ClearControls();
                    cancelPreRegistrationView = new CancelPreRegistrationView( this )
                                                    {
                                                        Dock = DockStyle.Fill,
                                                        SelectedAccount = account
                                                    };
                    Controls.Add( cancelPreRegistrationView );
                }
            }
            btnCancelRegistration.Enabled = true;
            Cursor = Cursors.Default;
        }

        private void btnConvertToShortPrereg_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            SelectedAccountProxy = GetLatestAccountProxyForSelectedAccount();

            if (SelectedAccountProxy == null)
            {
                Cursor = Cursors.Default;
                return;
            }

            // ensure that the account status has not changed since the user retrieved the account
            // 1. if account is no longer PreReg
            // 2. if account is locked

            if (SelectedAccountProxy.KindOfVisit.Code != VisitType.PREREG_PATIENT)
            {
                MessageBox.Show(UIErrorMessages.PATIENT_ACCTS_NOT_PREREG, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);

                RefreshCurrentWorklist();
                Cursor = Cursors.Default;
                return;
            }

            if (AccountLockStatus.IsAccountLocked(SelectedAccountProxy, User.GetCurrent()))
            {
                Cursor = Cursors.Default;
                MessageBox.Show(UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                RefreshCurrentWorklist();
            }
            else
            {
                SelectedAccountProxy.Activity = new ShortMaintenanceActivity { AssociatedActivityType = typeof(ShortRegistrationActivity) };
                // Pro-actively lock the account proxy
                if (!SelectedAccountProxy.Activity.ReadOnlyAccount())
                {
                    bool blnLocked = AccountActivityService.PlaceLockOn(SelectedAccountProxy, String.Empty);
                    if (!blnLocked)
                    {
                        RefreshCurrentWorklist();
                        Cursor = Cursors.Default;
                        btnConvertToShortPrereg.Enabled = true;
                        return;
                    }
                }
                //***************************************************************************************

                IAccount account = SelectedAccountProxy;
                account.IsShortRegistered = true;
                Cursor = Cursors.Default;

                AccountView.IsShortRegAccount = true;
                account.Activity = new ShortPreRegistrationActivity { AssociatedActivityType = typeof(MaintenanceActivity) };
                // Verify the account has not already been canceled
                if (account.IsCanceled)
                {
                    MessageBox.Show(ACCOUNT_CANCELED_MSG, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1);

                    RefreshCurrentWorklist();
                }
                else
                {
                    CompositeAction action = listView.SelectedItems[0].Tag as CompositeAction;
                    action.Context = account;
                    action.Execute();
                    ViewFactory.Instance.CreateView<PatientAccessView>().ActivateTab(action.Context.ToString(), account);
                    Cursor = Cursors.Default;
                }
            }
            btnConvertToShortPrereg.Enabled = true;
            Dispose();
        }

        private void btnEditAccount_Click( object sender, EventArgs e )
        {
            GetSelectedAccount();
        }

        private void worklistActionList_DoubleClick( object sender, EventArgs e )
        {
            GetSelectedAccount();
        }

        private void worklistActionList_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter )
            {
                GetSelectedAccount();
            }
        }

        private void AccountNameEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            lblAccountName.Text = args.Context as string;
        }
        /// <summary>
        /// Event handler to catch the Worklist object that the user clicked on in the
        /// Accounts ListView on the worklist view.  This event handler and 
        /// WorklistSelectedIndexEventHandler are fired in series since both the Worklist
        /// and the AccountProxy are required.
        /// </summary>
        private void WorklistEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            worklist = args.Context as Worklist;
        }

        /// <summary>
        /// Event handler to catch the AccountProxy that the user clicked on in the 
        /// Accounts ListView on the worklist view.  This event handler and 
        /// WorklistEventHandler are fired in series since both the Worklist
        /// and the AccountProxy are required.
        /// </summary>
        private void WorklistSelectedIndexEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            accountProxy = args.Context as AccountProxy;
            btnConvertToShortPrereg.Enabled = false;
            if ( accountProxy != null )
            {
                SelectedAccountProxy = accountProxy;

                actionCollection = ( ActionsList )accountProxy.GetRemainingActionsFor( worklist );
                //Enable btnConvertToShortPrereg
                if (SelectedAccountProxy.KindOfVisit.Code == VisitType.PREREG_PATIENT &&
                    SelectedAccountProxy.DerivedVisitType != Account.PRE_PUR &&
                    SelectedAccountProxy.DerivedVisitType != Account.PND_PUR &&
                    SelectedAccountProxy.IsShortRegistered == false &&
                    SelectedAccountProxy.IsQuickRegistered == false &&
                     SelectedAccountProxy.IsPAIWalkinRegistered == false &&
                    SelectedAccountProxy.IsNewBorn ==false &&
                    !SelectedAccountProxy.IsCanceledPreRegistration &&
                    !SelectedAccountProxy.IsCanceled)
                {
                    btnConvertToShortPrereg.Enabled = !preRegWorklistView.AccountIsLocked;
                }

                //Enable btnActivateShortAccount
                if ( SelectedAccountProxy.IsNewBorn)
                {
                    btnActivateShortAccount.Enabled = false;
                }
                else
                {
                    btnActivateShortAccount.Enabled = !noShowWorklistView.AccountIsLocked; 
                }
                if ( actionCollection != null )
                {   // Display the CompositeAction objects in the ListView
                    PopulateActionsList();
                }
                else
                {
                    listView.Items.Clear();
                    ListViewItem lvi = new ListViewItem { Text = "No action items to display" };
                    btnEditAccount.Enabled = false;

                    if ( btnCancelRegistration.Visible )
                    {
                        btnCancelRegistration.Enabled = false;
                    }
                    if ( btnEditAccount.Visible )
                    {
                        btnEditAccount.Enabled = false;
                    }
                    listView.Items.Add( lvi );
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// SetTabPage() method.
        /// The worklists must remain in memory once loaded, so this method enables the
        /// menu handler to set the active tab page index if the worklist form is loaded.
        /// </summary>
        public void SetTabPage( int selectedScreenIndex )
        {
            tabAccounts.SelectedIndex = selectedScreenIndex;
        }

        private void RefreshCurrentWorklist()
        {
            switch ( worklist.ToString().ToUpper() )
            {
                case "NO SHOW":
                    {
                        btnActivateAccount.Enabled = false;
                        btnActivateShortAccount.Enabled = false;
                        btnCancelRegistration.Enabled = false;
                        btnEditAccount.Enabled = false;
                        noShowWorklistView.RefreshWorklistResults();
                        break;
                    }
                case "PREREGISTRATION":
                    {
                        btnEditAccount.Enabled = false;
                        btnConvertToShortPrereg.Enabled = false;
                        preRegWorklistView.RefreshWorklistResults();
                        break;
                    }
                case "POSTREGISTRATION":
                    {
                        btnEditAccount.Enabled = false;
                        postRegWorklistView.RefreshWorklistResults();
                        break;
                    }
                case "PATIENT LIABILITY":
                    {
                        btnEditAccount.Enabled = false;
                        liabilityWorklistView.RefreshWorklistResults();
                        break;
                    }
                case "INSURANCE VERIFICATION":
                    {
                        btnEditAccount.Enabled = false;
                        insuranceWorklistView.RefreshWorklistResults();
                        break;
                    }
                case "PRE-MSE":
                    {
                        btnEditAccount.Enabled = false;
                        preMSEWorklistView.RefreshWorklistResults();
                        break;
                    }
            }

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// </summary>
        private void SetDefaultButtonState()
        {
            btnCancelRegistration.Visible = false;
            btnActivateAccount.Visible = false;
            btnActivateShortAccount.Visible = false;
            btnConvertToShortPrereg.Visible = false;
            btnEditAccount.Enabled = false;
            btnEditAccount.Visible = true;
            btnEditAccount.Text = "Edit/Maintain &Account...";
            btnEditAccount.Location = new Point( 887, 584 );
            btnEditAccount.Size = new Size( 132, 23 );
            btnEditAccount.TabIndex = 2;
        }

        private void SetPreregistrationButtonState()
        {
            btnCancelRegistration.Visible = false;
            btnActivateAccount.Visible = false;
            btnActivateShortAccount.Visible = false;
            btnConvertToShortPrereg.Visible = true;
            btnConvertToShortPrereg.Location = new Point(750,578 );
            btnConvertToShortPrereg.TabIndex = 2;
            btnEditAccount.Enabled = false;
            btnEditAccount.Visible = true;
            btnEditAccount.Text = "Edit/Maintain &Account...";
            btnEditAccount.Location = new Point(887, 584);
            btnEditAccount.Size = new Size(132, 23);
            btnEditAccount.TabIndex = 3;
        }

        private void SetWalkinButtonState()
        {
            btnCancelRegistration.Visible = true;
            btnCancelRegistration.Enabled = true;

            btnActivateAccount.Visible = true;
            btnActivateAccount.Enabled = true;

            btnActivateShortAccount.Visible = true;
            btnActivateShortAccount.Enabled = true;

            btnConvertToShortPrereg.Visible = false;
            btnConvertToShortPrereg.Enabled = false;
            btnEditAccount.Enabled = false;
            btnEditAccount.Visible = false;
        }

        private void SetPreMseButtonState()
        {
            btnCancelRegistration.Visible = false;
            btnActivateAccount.Visible = false;
            btnActivateShortAccount.Visible = false;
            btnConvertToShortPrereg.Visible = false;
            btnEditAccount.Enabled = false;
            btnEditAccount.Visible = true;
            btnEditAccount.Text = "Comp&lete Post-MSE Registration...";
            btnEditAccount.Location = new Point( 830, 584 );
            btnEditAccount.Size = new Size( 189, 23 );
            btnEditAccount.TabIndex = 2;
        }

        private void SetNoShowButtonState()
        {
            btnEditAccount.Enabled = false;
            btnEditAccount.Visible = true;
            btnEditAccount.Text = "Edit/Maintain &Account...";
            btnEditAccount.Location = new Point( 435, 584 );
            btnEditAccount.Size = new Size( 132, 23 );
            btnEditAccount.TabIndex = 2;
            btnCancelRegistration.Visible = true;
            btnCancelRegistration.Enabled = false;
            btnCancelRegistration.TabIndex = 3;
            btnActivateAccount.Enabled = false;
            btnActivateAccount.Visible = true;
            btnActivateAccount.TabIndex = 4;
            btnActivateShortAccount.Enabled = false;
            btnActivateShortAccount.Visible = true;
            btnActivateShortAccount.TabIndex = 5;
            btnConvertToShortPrereg.Visible = false;
        }

        /// <summary>
        /// UpdateView() method.
        /// </summary>
        public override void UpdateView()
        {
            listView.Items.Clear();
            lblAccountName.Text = String.Empty;

            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
            Facility securityFrameworkFacility =
                new Facility( patientAccessUser.Facility.Code,
                patientAccessUser.Facility.Description );
            bool canViewWorklist = false;

            if ( tabAccounts.SelectedIndex > 0 )
            {
                BreadCrumbLogger.GetInstance.Log( ( ( ScreenIndexes )tabAccounts.SelectedIndex ) + " tab selected" );
            }

            selectedTab = ( ScreenIndexes )tabAccounts.SelectedIndex;
            switch ( selectedTab )
            {
                case ScreenIndexes.PREREGISTRATION:
                    canViewWorklist = securityUser.HasPermissionTo( Privilege.Actions.View,
                        new PreRegistrationWorklistActivity().Description, securityFrameworkFacility );
                    if ( canViewWorklist )
                    {
                        SetVisibilityFor( panel.Controls, true );
                        //SetDefaultButtonState();
                        SetPreregistrationButtonState();
                        preRegWorklistView.UpdateView();
                    }
                    else
                    {
                        SetVisibilityFor( panel.Controls, false );
                        SetNotVisibleButtonState();
                        ClearWorkListTab( tabPreRegistration );
                    }
                    break;

                case ScreenIndexes.POSTREGISTRATION:
                    canViewWorklist = securityUser.HasPermissionTo( Privilege.Actions.View,
                        new PostRegistrationWorklistActivity().Description, securityFrameworkFacility );
                    if ( canViewWorklist )
                    {
                        SetVisibilityFor( panel.Controls, true );
                        SetDefaultButtonState();
                        postRegWorklistView.UpdateView();
                    }
                    else
                    {
                        SetVisibilityFor( panel.Controls, false );
                        SetNotVisibleButtonState();
                        ClearWorkListTab( tabPostRegistration );
                    }
                    break;

                case ScreenIndexes.INSURANCE:
                    canViewWorklist = securityUser.HasPermissionTo( Privilege.Actions.View,
                        new InsuranceVerificationWorklistActivity().Description, securityFrameworkFacility );
                    if ( canViewWorklist )
                    {
                        SetVisibilityFor( panel.Controls, true );
                        SetDefaultButtonState();
                        insuranceWorklistView.UpdateView();
                    }
                    else
                    {
                        SetVisibilityFor( panel.Controls, false );
                        SetNotVisibleButtonState();
                        ClearWorkListTab( tabInsurance );
                    }
                    break;

                case ScreenIndexes.LIABILITY:
                    canViewWorklist = securityUser.HasPermissionTo( Privilege.Actions.View,
                        new PatientLiabilityWorklistActivity().Description, securityFrameworkFacility );
                    if ( canViewWorklist )
                    {
                        SetVisibilityFor( panel.Controls, true );
                        SetDefaultButtonState();
                        liabilityWorklistView.UpdateView();
                    }
                    else
                    {
                        SetVisibilityFor( panel.Controls, false );
                        SetNotVisibleButtonState();
                        ClearWorkListTab( tabLiability );
                    }
                    break;

                case ScreenIndexes.PREMSE:
                    canViewWorklist = securityUser.HasPermissionTo( Privilege.Actions.View,
                        new PreMSEWorklistActivity().Description, securityFrameworkFacility );
                    if ( canViewWorklist )
                    {
                        SetVisibilityFor( panel.Controls, true );
                        SetPreMseButtonState();
                        preMSEWorklistView.UpdateView();
                    }
                    else
                    {
                        SetVisibilityFor( panel.Controls, false );
                        SetNotVisibleButtonState();
                        ClearWorkListTab( tabPreMSE );
                    }
                    break;

                case ScreenIndexes.NOSHOW:
                    canViewWorklist = securityUser.HasPermissionTo( Privilege.Actions.View,
                        new NoShowWorklistActivity().Description, securityFrameworkFacility );

                    if ( canViewWorklist )
                    {
                        SetVisibilityFor( panel.Controls, true );
                        SetNoShowButtonState();
                        noShowWorklistView.UpdateView();
                    }
                    else
                    {
                        SetVisibilityFor( panel.Controls, false );
                        SetNotVisibleButtonState();
                        ClearWorkListTab( tabNoShow );
                    }
                    break;
            }
        }
        #endregion

        #region Properties
        public Account Model_Account
        {
            get
            {
                return ( Account )Model;
            }
        }

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

        public ListView ListView
        {
            get
            {
                return listView;
            }
        }
        #endregion

        #region Private Methods
        private static void SetVisibilityFor( ControlCollection controls, bool isVisible )
        {
            foreach ( Control c in controls )
            {
                c.Visible = isVisible;
            }
        }

        private void ClearControls()
        {
            foreach ( Control control in Controls )
            {
                if ( control != null )
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch ( Exception ex )
                    {
                        c_log.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }
            Controls.Clear();
        }

        private static void ClearWorkListTab( TabPage tab )
        {
            foreach ( Control control in tab.Controls )
            {
                if ( control != null )
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch ( Exception ex )
                    {
                        c_log.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }
            tab.Controls.Clear();
            tab.Refresh();

            IncompleteWorklistView incompleteWorklistView = new IncompleteWorklistView();
            tab.Controls.Add( incompleteWorklistView );
            incompleteWorklistView.UpdateView();
        }

        private void SetNotVisibleButtonState()
        {
            btnCancelRegistration.Visible = false;
            btnActivateAccount.Visible = false;
            btnActivateShortAccount.Visible = false;
            btnEditAccount.Visible = false;
            btnConvertToShortPrereg.Visible = false;
        }

        public void GetSelectedAccount()
        {
            Cursor = Cursors.WaitCursor;

            if ( AccountLockStatus.IsAccountLocked( SelectedAccountProxy, User.GetCurrent() ) )
            {
                MessageBox.Show( UIErrorMessages.NO_EDIT_RECORD_LOCKED, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
                RefreshCurrentWorklist();
                Cursor = Cursors.Default;
                return;
            }

            bool blnPreReg = false;

            if ( IsActionSelected() )
            {
                switch ( ( ScreenIndexes )tabAccounts.SelectedIndex )
                {
                    case ScreenIndexes.PREREGISTRATION:
                        blnPreReg = true;
                        break;
                }

                CompositeAction action = listView.SelectedItems[0].Tag as CompositeAction;

                AccountProxy ap = null;
                IAccount theAccount = null;

                IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

                ap = broker.AccountProxyFor(
                        User.GetCurrent().Facility.Code,
                        SelectedAccountProxy.Patient.MedicalRecordNumber,
                        SelectedAccountProxy.AccountNumber );

                SelectedAccountProxy = ap;

                // force retrieval of patient

                ap.Patient.Oid = -1;

                if( ap.IsShortRegisteredNonDayCareAccount() )
                {
                    AccountView.IsShortRegAccount = true;
                    ap.Activity = new ShortMaintenanceActivity();
                }
                else if ( ap.IsQuickPreRegAccount() )
                {
                    AccountView.IsShortRegAccount = false;
                    AccountView.IsQuickRegistered = true;
                    ap.Activity = new QuickAccountMaintenanceActivity();
                }
                else
                {
                    AccountView.IsShortRegAccount = false;
                    ap.Activity = new MaintenanceActivity();
                }

                theAccount = ap;

                //PRE-MSE Verification
                bool blnPreMSEAccount = (theAccount.FinancialClass.Code.Equals("37") &&
                                         (theAccount.KindOfVisit.IsEmergencyPatient ||
                                          (theAccount.KindOfVisit.IsOutpatient )));

                if ( ( ScreenIndexes )tabAccounts.SelectedIndex == ScreenIndexes.PREMSE &&
                     !blnPreMSEAccount )
                {   // Account is no longer in Pre-MSE state
                    MessageBox.Show( NOT_PRE_MSE_ACCOUNT_MSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                    RefreshCurrentWorklist();
                    Cursor = Cursors.Default;
                    return;
                }

                if ( theAccount.IsNewBorn && theAccount.KindOfVisit != null && theAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT )
                {
                    theAccount.Activity.AssociatedActivityType = typeof( PreAdmitNewbornActivity );
                }
                else
                {
                    if ( blnPreReg )
                    {
                        theAccount.Activity.AssociatedActivityType = ap.IsShortRegisteredNonDayCareAccount()
                            ? typeof( ShortPreRegistrationActivity ) : typeof( PreRegistrationActivity );
                    }
                    else
                    {
                        theAccount.Activity.AssociatedActivityType = ap.IsShortRegisteredNonDayCareAccount()
                            ? typeof( ShortRegistrationActivity ) : typeof( RegistrationActivity );
                    }
                }

                // whack the ins and FC if this is a post mse
                if ( blnPreMSEAccount )
                {
                    
                    if (theAccount.IsUrgentCarePreMse )
                    {
                        theAccount.Activity = new UCCPostMseRegistrationActivity();
                    }
                    else
                    {
                        theAccount.Activity = new PostMSERegistrationActivity();
                    }
                        
           
                    theAccount.FinancialClass = new FinancialClass();
                }

                //***************************************************************************************

                // Pro-actively lock the account

                if ( !theAccount.Activity.ReadOnlyAccount() )
                {
                    bool blnLocked = AccountActivityService.PlaceLockOn( theAccount, String.Empty );

                    if ( !blnLocked )
                    {
                        RefreshCurrentWorklist();
                        btnEditAccount.Enabled = true;
                        return;
                    }
                }

                //***************************************************************************************
              
                ((PatientAccessView)this.Parent.Parent).SelectedAccount = theAccount;

                BreadCrumbLogger.GetInstance.Log("Account selected ", theAccount as AccountProxy);

                if (action == null)
                {
                    IAction leaf = listView.SelectedItems[0].Tag as IAction;
                    if ( leaf != null )
                    {
                        leaf.Context = theAccount;
                        leaf.Execute();

                        ViewFactory.Instance.CreateView<PatientAccessView>().ActivateTab( leaf.Context.ToString(), theAccount );
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    action.Context = theAccount;
                    action.Execute();

                    ViewFactory.Instance.CreateView<PatientAccessView>().ActivateTab( action.Context.ToString(), theAccount );
                    Cursor = Cursors.Default;
                }
            }
            btnEditAccount.Enabled = true;
            Dispose();
        }

        private bool IsActionSelected()
        {
            foreach ( ListViewItem item in listView.Items )
            {
                if ( item.Selected )
                {
                    BreadCrumbLogger.GetInstance.Log( item.Text + " action selected" );
                    return true;
                }
            }
            return false;
        }

        private void PopulateActionsList()
        {
            try
            {
                btnEditAccount.Enabled = false;
                btnCancelRegistration.Enabled = false;
                btnEditAccount.Enabled = false;

                Cursor = Cursors.AppStarting;
                listView.Items.Clear();
                listView.BeginUpdate();

                foreach ( IAction action in actionCollection )
                {
                    if ( action != null )
                    {
                        ListViewItem lvi = new ListViewItem { Tag = action, Text = action.Description };

                        int count = 0;
                        if ( action is CompositeAction )
                        {
                            CompositeAction ca = ( CompositeAction )action;
                            count += ca.NumberOfAllLeafActions();
                        }
                        else
                        {   // It's a LeafAction
                            count++;
                        }
                        lvi.SubItems.Add( count.ToString() );
                        listView.Items.Add( lvi );
                    }
                }
                actionItemsToDisplay = ( listView.Items.Count > 0 );

                if ( listView.Items.Count > 0 )
                {
                    switch ( tabAccounts.SelectedIndex )
                    {
                        case ( int )ScreenIndexes.PREREGISTRATION:
                            SetButtonState();
                            btnEditAccount.Enabled = !preRegWorklistView.AccountIsLocked;
                            btnActivateShortAccount.Enabled = !preRegWorklistView.AccountIsLocked && accountProxy.IsPAIWalkinRegistered ;
                            btnActivateAccount.Enabled = !preRegWorklistView.AccountIsLocked && accountProxy.IsPAIWalkinRegistered;
                            btnCancelRegistration.Enabled = !preRegWorklistView.AccountIsLocked && accountProxy.IsPAIWalkinRegistered;
                            break;

                        case ( int )ScreenIndexes.POSTREGISTRATION:
                            btnEditAccount.Enabled = !postRegWorklistView.AccountIsLocked;
                            break;

                        case ( int )ScreenIndexes.INSURANCE:
                            btnEditAccount.Enabled = !insuranceWorklistView.AccountIsLocked;
                            break;

                        case ( int )ScreenIndexes.LIABILITY:
                            btnEditAccount.Enabled = !liabilityWorklistView.AccountIsLocked;
                            break;

                        case ( int )ScreenIndexes.PREMSE:
                            btnEditAccount.Enabled = !preMSEWorklistView.AccountIsLocked;
                            break;

                        case ( int )ScreenIndexes.NOSHOW:

                            bool canActivateAccount = false;
                            User patientAccessUser = User.GetCurrent();
                            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
                            Facility securityFrameworkFacility = new Facility( patientAccessUser.Facility.Code, patientAccessUser.Facility.Description );

                            canActivateAccount = securityUser.HasPermissionTo( Privilege.Actions.Activate, typeof( Account ), securityFrameworkFacility );
                            if ( canActivateAccount )
                            {
                                btnActivateAccount.Enabled = !noShowWorklistView.AccountIsLocked;
                                if ( btnActivateShortAccount.Enabled )
                                    btnActivateShortAccount.Enabled = !noShowWorklistView.AccountIsLocked;
                            }
                            else
                            {
                                btnActivateAccount.Enabled = false;
                                btnActivateShortAccount.Enabled = false;
                            }
                            btnCancelRegistration.Enabled = !noShowWorklistView.AccountIsLocked;
                            btnEditAccount.Enabled = !noShowWorklistView.AccountIsLocked && !accountProxy.IsPAIWalkinRegistered;
                            break;
                    }
                    listView.Items[0].Selected = true;
                }
                else
                {
                    listView.Items.Clear();
                    ListViewItem lvi = new ListViewItem { Text = "No action items to display" };
                    btnEditAccount.Enabled = false;
                    listView.Items.Add( lvi );
                }
            }
            finally
            {
                listView.EndUpdate();
                Cursor = Cursors.Default;
            }
        }

        private void SetButtonState()
        {
            if (SelectedAccountProxy.IsPAIWalkinRegistered)
            {
                SetWalkinButtonState();
            }
            else
            {
                SetPreregistrationButtonState();
            }
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

                        NMHDR nm = new NMHDR();
                        nm.hwndFrom = IntPtr.Zero;
                        nm.idFrom = 0;
                        nm.code = 0;

                        int idCtrl = ( int )m.WParam;
                        NMHDR nmh = ( NMHDR )m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            bool rc = checkForError();
                            int irc = 0;
                            if ( rc )
                            {
                                irc = 1;
                            }

                            Convert.ToInt32( rc );
                            m.Result = ( IntPtr )irc;
                        }
                        break;
                    }
                default:
                    break;

            }
        }

        private static bool checkForError()
        {
            bool rcErrors = false;

            rcErrors = RuleEngine.GetInstance().AccountHasFailedError();

            return rcErrors;
        }

        public void ResetControls()
        {
            lblAccountName.Text = String.Empty;
            listView.Items.Clear();
            btnEditAccount.Enabled = false;
            btnConvertToShortPrereg.Enabled = false;
            btnActivateAccount.Enabled = false;
            btnActivateShortAccount.Enabled = false;
            btnCancelRegistration.Enabled = false;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.panel = new System.Windows.Forms.Panel();
            this.lblItems = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHdrAction = new System.Windows.Forms.ColumnHeader();
            this.columnHdrCount = new System.Windows.Forms.ColumnHeader();
            this.lblAccountName = new System.Windows.Forms.Label();
            this.lblToDoListFor = new System.Windows.Forms.Label();
            this.tabAccounts = new System.Windows.Forms.TabControl();
            this.tabPreRegistration = new System.Windows.Forms.TabPage();
            this.preRegWorklistView = new PatientAccess.UI.WorklistViews.PreRegWorklistView( this );
            this.tabPostRegistration = new System.Windows.Forms.TabPage();
            this.postRegWorklistView = new PatientAccess.UI.WorklistViews.PostRegWorklistView( this );
            this.tabInsurance = new System.Windows.Forms.TabPage();
            this.insuranceWorklistView = new PatientAccess.UI.WorklistViews.InsuranceWorklistView( this );
            this.tabLiability = new System.Windows.Forms.TabPage();
            this.liabilityWorklistView = new PatientAccess.UI.WorklistViews.LiabilityWorklistView( this );
            this.tabPreMSE = new System.Windows.Forms.TabPage();
            this.preMSEWorklistView = new PatientAccess.UI.WorklistViews.PreMSEWorklistView( this );
            this.tabNoShow = new System.Windows.Forms.TabPage();
            this.noShowWorklistView = new PatientAccess.UI.WorklistViews.NoShowWorklistView( this );
            this.btnActivateAccount = new ClickOnceLoggingButton();
            this.btnActivateShortAccount = new ClickOnceLoggingButton();
            this.btnCancelRegistration = new ClickOnceLoggingButton();
            this.btnEditAccount = new ClickOnceLoggingButton();
            this.btnConvertToShortPrereg = new ClickOnceLoggingButton();
            this.panel.SuspendLayout();
            this.tabAccounts.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 128 ) ) ) ), ( ( int )( ( ( byte )( 162 ) ) ) ), ( ( int )( ( ( byte )( 200 ) ) ) ) );
            this.contextLabel.Description = " Worklists";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.ForeColor = System.Drawing.Color.White;
            this.contextLabel.Location = new System.Drawing.Point( 0, 0 );
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size( 1024, 23 );
            this.contextLabel.TabIndex = 0;
            this.contextLabel.TabStop = false;
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.White;
            this.panel.Controls.Add( this.lblItems );
            this.panel.Controls.Add( this.listView );
            this.panel.Controls.Add( this.lblAccountName );
            this.panel.Controls.Add( this.lblToDoListFor );
            this.panel.Location = new System.Drawing.Point( 7, 337 );
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size( 1010, 235 );
            this.panel.TabIndex = 0;
            // 
            // lblItems
            // 
            this.lblItems.ForeColor = System.Drawing.Color.GhostWhite;
            this.lblItems.Location = new System.Drawing.Point( 920, 12 );
            this.lblItems.Name = "lblItems";
            this.lblItems.Size = new System.Drawing.Size( 72, 23 );
            this.lblItems.TabIndex = 2;
            this.lblItems.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listView
            // 
            this.listView.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHdrAction,
            this.columnHdrCount} );
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point( 10, 48 );
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size( 988, 175 );
            this.listView.TabIndex = 1;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler( this.worklistActionList_KeyDown );
            this.listView.DoubleClick += new System.EventHandler( this.worklistActionList_DoubleClick );
            // 
            // columnHdrAction
            // 
            this.columnHdrAction.Text = "Action Item";
            this.columnHdrAction.Width = 400;
            // 
            // columnHdrCount
            // 
            this.columnHdrCount.Text = "Count";
            this.columnHdrCount.Width = 75;
            // 
            // lblAccountName
            // 
            this.lblAccountName.Location = new System.Drawing.Point( 96, 13 );
            this.lblAccountName.Name = "lblAccountName";
            this.lblAccountName.Size = new System.Drawing.Size( 808, 23 );
            this.lblAccountName.TabIndex = 0;
            this.lblAccountName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblToDoListFor
            // 
            this.lblToDoListFor.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.lblToDoListFor.Location = new System.Drawing.Point( 10, 13 );
            this.lblToDoListFor.Name = "lblToDoListFor";
            this.lblToDoListFor.Size = new System.Drawing.Size( 88, 23 );
            this.lblToDoListFor.TabIndex = 0;
            this.lblToDoListFor.Text = "To Do List for:";
            this.lblToDoListFor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabAccounts
            // 
            this.tabAccounts.Controls.Add( this.tabPreRegistration );
            this.tabAccounts.Controls.Add( this.tabPostRegistration );
            this.tabAccounts.Controls.Add( this.tabInsurance );
            this.tabAccounts.Controls.Add( this.tabLiability );
            this.tabAccounts.Controls.Add( this.tabPreMSE );
            this.tabAccounts.Controls.Add( this.tabNoShow );
            this.tabAccounts.Location = new System.Drawing.Point( 7, 30 );
            this.tabAccounts.Name = "tabAccounts";
            this.tabAccounts.SelectedIndex = 0;
            this.tabAccounts.Size = new System.Drawing.Size( 1010, 300 );
            this.tabAccounts.TabIndex = 0;
            this.tabAccounts.SelectedIndexChanged += new System.EventHandler( this.tabAccounts_SelectedIndexChanged );
            // 
            // tabPreRegistration
            // 
            this.tabPreRegistration.BackColor = System.Drawing.Color.White;
            this.tabPreRegistration.Controls.Add( this.preRegWorklistView );
            this.tabPreRegistration.Location = new System.Drawing.Point( 4, 22 );
            this.tabPreRegistration.Name = "tabPreRegistration";
            this.tabPreRegistration.Size = new System.Drawing.Size( 1002, 274 );
            this.tabPreRegistration.TabIndex = 0;
            this.tabPreRegistration.Text = "Preregistration";
            // 
            // preRegWorklistView
            // 
            this.preRegWorklistView.AccountIsLocked = false;
            this.preRegWorklistView.BackColor = System.Drawing.Color.White;
            this.preRegWorklistView.HeaderText = "Preregistration Worklist";
            this.preRegWorklistView.Location = new System.Drawing.Point( 0, 0 );
            this.preRegWorklistView.Model = null;
            this.preRegWorklistView.Name = "preRegWorklistView";
            this.preRegWorklistView.Size = new System.Drawing.Size( 1000, 300 );
            this.preRegWorklistView.TabIndex = 0;
            this.preRegWorklistView.WorklistType = 0;
            this.preRegWorklistView.WorklistEvent += new System.EventHandler( this.WorklistEventHandler );
            this.preRegWorklistView.AccountNameEvent += new System.EventHandler( this.AccountNameEventHandler );
            this.preRegWorklistView.ResetActionsButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.preRegWorklistView.WorklistSelectedIndexEvent += new System.EventHandler( this.WorklistSelectedIndexEventHandler );
            // 
            // tabPostRegistration
            // 
            this.tabPostRegistration.BackColor = System.Drawing.Color.White;
            this.tabPostRegistration.Controls.Add( this.postRegWorklistView );
            this.tabPostRegistration.Location = new System.Drawing.Point( 4, 22 );
            this.tabPostRegistration.Name = "tabPostRegistration";
            this.tabPostRegistration.Size = new System.Drawing.Size( 1002, 274 );
            this.tabPostRegistration.TabIndex = 1;
            this.tabPostRegistration.Text = "Postregistration";
            // 
            // postRegWorklistView
            // 
            this.postRegWorklistView.AccountIsLocked = false;
            this.postRegWorklistView.BackColor = System.Drawing.Color.White;
            this.postRegWorklistView.HeaderText = "Postregistration Worklist";
            this.postRegWorklistView.Location = new System.Drawing.Point( 0, 0 );
            this.postRegWorklistView.Model = null;
            this.postRegWorklistView.Name = "postRegWorklistView";
            this.postRegWorklistView.Size = new System.Drawing.Size( 1000, 300 );
            this.postRegWorklistView.TabIndex = 0;
            this.postRegWorklistView.WorklistType = 1;
            this.postRegWorklistView.WorklistEvent += new System.EventHandler( this.WorklistEventHandler );
            this.postRegWorklistView.AccountNameEvent += new System.EventHandler( this.AccountNameEventHandler );
            this.postRegWorklistView.ResetActionsButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.postRegWorklistView.WorklistSelectedIndexEvent += new System.EventHandler( this.WorklistSelectedIndexEventHandler );
            // 
            // tabInsurance
            // 
            this.tabInsurance.BackColor = System.Drawing.Color.White;
            this.tabInsurance.Controls.Add( this.insuranceWorklistView );
            this.tabInsurance.Location = new System.Drawing.Point( 4, 22 );
            this.tabInsurance.Name = "tabInsurance";
            this.tabInsurance.Size = new System.Drawing.Size( 1002, 274 );
            this.tabInsurance.TabIndex = 2;
            this.tabInsurance.Text = "Insurance";
            // 
            // insuranceWorklistView
            // 
            this.insuranceWorklistView.AccountIsLocked = true;
            this.insuranceWorklistView.BackColor = System.Drawing.Color.White;
            this.insuranceWorklistView.HeaderText = "Insurance Verification Worklist";
            this.insuranceWorklistView.Location = new System.Drawing.Point( 0, 0 );
            this.insuranceWorklistView.Model = null;
            this.insuranceWorklistView.Name = "insuranceWorklistView";
            this.insuranceWorklistView.Size = new System.Drawing.Size( 1000, 300 );
            this.insuranceWorklistView.TabIndex = 0;
            this.insuranceWorklistView.WorklistType = 2;
            this.insuranceWorklistView.WorklistEvent += new System.EventHandler( this.WorklistEventHandler );
            this.insuranceWorklistView.AccountNameEvent += new System.EventHandler( this.AccountNameEventHandler );
            this.insuranceWorklistView.ResetActionsButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.insuranceWorklistView.WorklistSelectedIndexEvent += new System.EventHandler( this.WorklistSelectedIndexEventHandler );
            // 
            // tabLiability
            // 
            this.tabLiability.BackColor = System.Drawing.Color.White;
            this.tabLiability.Controls.Add( this.liabilityWorklistView );
            this.tabLiability.Location = new System.Drawing.Point( 4, 22 );
            this.tabLiability.Name = "tabLiability";
            this.tabLiability.Size = new System.Drawing.Size( 1002, 274 );
            this.tabLiability.TabIndex = 3;
            this.tabLiability.Text = "Liability";
            // 
            // liabilityWorklistView
            // 
            this.liabilityWorklistView.AccountIsLocked = true;
            this.liabilityWorklistView.BackColor = System.Drawing.Color.White;
            this.liabilityWorklistView.HeaderText = "Patient Liability Worklist";
            this.liabilityWorklistView.Location = new System.Drawing.Point( 0, 0 );
            this.liabilityWorklistView.Model = null;
            this.liabilityWorklistView.Name = "liabilityWorklistView";
            this.liabilityWorklistView.Size = new System.Drawing.Size( 1000, 300 );
            this.liabilityWorklistView.TabIndex = 0;
            this.liabilityWorklistView.WorklistType = 3;
            this.liabilityWorklistView.WorklistEvent += new System.EventHandler( this.WorklistEventHandler );
            this.liabilityWorklistView.AccountNameEvent += new System.EventHandler( this.AccountNameEventHandler );
            this.liabilityWorklistView.ResetActionsButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.liabilityWorklistView.WorklistSelectedIndexEvent += new System.EventHandler( this.WorklistSelectedIndexEventHandler );
            // 
            // tabPreMSE
            // 
            this.tabPreMSE.BackColor = System.Drawing.Color.White;
            this.tabPreMSE.Controls.Add( this.preMSEWorklistView );
            this.tabPreMSE.Location = new System.Drawing.Point( 4, 22 );
            this.tabPreMSE.Name = "tabPreMSE";
            this.tabPreMSE.Size = new System.Drawing.Size( 1002, 274 );
            this.tabPreMSE.TabIndex = 4;
            this.tabPreMSE.Text = "Pre-MSE";
            // 
            // preMSEWorklistView
            // 
            this.preMSEWorklistView.AccountIsLocked = true;
            this.preMSEWorklistView.BackColor = System.Drawing.Color.White;
            this.preMSEWorklistView.HeaderText = "Emergency Department Worklist";
            this.preMSEWorklistView.Location = new System.Drawing.Point( 0, 0 );
            this.preMSEWorklistView.Model = null;
            this.preMSEWorklistView.Name = "preMSEWorklistView";
            this.preMSEWorklistView.Size = new System.Drawing.Size( 1000, 300 );
            this.preMSEWorklistView.TabIndex = 0;
            this.preMSEWorklistView.WorklistType = 4;
            this.preMSEWorklistView.WorklistEvent += new System.EventHandler( this.WorklistEventHandler );
            this.preMSEWorklistView.AccountNameEvent += new System.EventHandler( this.AccountNameEventHandler );
            this.preMSEWorklistView.ResetActionsButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.preMSEWorklistView.WorklistSelectedIndexEvent += new System.EventHandler( this.WorklistSelectedIndexEventHandler );
            // 
            // tabNoShow
            // 
            this.tabNoShow.BackColor = System.Drawing.Color.White;
            this.tabNoShow.Controls.Add( this.noShowWorklistView );
            this.tabNoShow.Location = new System.Drawing.Point( 4, 22 );
            this.tabNoShow.Name = "tabNoShow";
            this.tabNoShow.Size = new System.Drawing.Size( 1002, 274 );
            this.tabNoShow.TabIndex = 5;
            this.tabNoShow.Text = "No Show";
            // 
            // noShowWorklistView
            // 
            this.noShowWorklistView.AccountIsLocked = false;
            this.noShowWorklistView.BackColor = System.Drawing.Color.White;
            this.noShowWorklistView.HeaderText = "No Show Worklist";
            this.noShowWorklistView.Location = new System.Drawing.Point( 0, 0 );
            this.noShowWorklistView.Model = null;
            this.noShowWorklistView.Name = "noShowWorklistView";
            this.noShowWorklistView.Size = new System.Drawing.Size( 1000, 300 );
            this.noShowWorklistView.TabIndex = 0;
            this.noShowWorklistView.WorklistType = 5;
            this.noShowWorklistView.WorklistEvent += new System.EventHandler( this.WorklistEventHandler );
            this.noShowWorklistView.AccountNameEvent += new System.EventHandler( this.AccountNameEventHandler );
            this.noShowWorklistView.ResetActionsButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.noShowWorklistView.WorklistSelectedIndexEvent += new System.EventHandler( this.WorklistSelectedIndexEventHandler );
            
            // 
            // btnActivateShortAccount
            // 
            this.btnActivateShortAccount.Location = new System.Drawing.Point(887, 578);
            this.btnActivateShortAccount.Message = null;
            this.btnActivateShortAccount.Name = "btnActivateShortAccount";
            this.btnActivateShortAccount.Size = new System.Drawing.Size(130, 34);
            this.btnActivateShortAccount.TabIndex = 0;
            this.btnActivateShortAccount.Text = "  Activate Account - &Diagnostic Registration";
            this.btnActivateShortAccount.Click += new System.EventHandler(this.btnActivateShortAccount_Click);
            // 
            // btnActivateAccount
            // 
            this.btnActivateAccount.Location = new System.Drawing.Point( 751, 578 );
            this.btnActivateAccount.Message = null;
            this.btnActivateAccount.Name = "btnActivateAccount";
            this.btnActivateAccount.Size = new System.Drawing.Size( 130, 34 );
            this.btnActivateAccount.TabIndex = 0;
            this.btnActivateAccount.Text = "  Acti&vate Account - Standard Registration";
            this.btnActivateAccount.Click += new System.EventHandler( this.btnActivateAccount_Click );
            // 
            // btnCancelRegistration
            // 
            this.btnCancelRegistration.Location = new System.Drawing.Point( 573, 584 );
            this.btnCancelRegistration.Message = null;
            this.btnCancelRegistration.Name = "btnCancelRegistration";
            this.btnCancelRegistration.Size = new System.Drawing.Size( 172, 23 );
            this.btnCancelRegistration.TabIndex = 0;
            this.btnCancelRegistration.Text = "Cance&l Preregistered Account...";
            this.btnCancelRegistration.Click += new System.EventHandler( this.btnCancelRegistration_Click );
            
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Enabled = false;
            this.btnEditAccount.Location = new System.Drawing.Point( 435, 584 );
            this.btnEditAccount.Message = null;
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.Size = new System.Drawing.Size( 132, 23 );
            this.btnEditAccount.TabIndex = 2;
            this.btnEditAccount.Text = "Edit/Maintain &Account...";
            this.btnEditAccount.Click += new System.EventHandler( this.btnEditAccount_Click );
            // 
            // btnConvertToShortPrereg
            // 
            this.btnConvertToShortPrereg.Enabled = false;
            this.btnConvertToShortPrereg.Location = new System.Drawing.Point(276, 584);
            this.btnConvertToShortPrereg.Message = null;
            this.btnConvertToShortPrereg.Name = "btnConvertToShortPrereg";
            this.btnConvertToShortPrereg.Size = new System.Drawing.Size(130, 34);
            this.btnConvertToShortPrereg.TabIndex = 0;
            this.btnConvertToShortPrereg.Text = "    Convert Account - Diagnostic Preregistration";
            this.btnConvertToShortPrereg.Click += new System.EventHandler(this.btnConvertToShortPrereg_Click);
            // 
            // WorklistsView
            // 
            this.BackColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 209 ) ) ) ), ( ( int )( ( ( byte )( 228 ) ) ) ), ( ( int )( ( ( byte )( 243 ) ) ) ) );
            this.Controls.Add( this.tabAccounts );
            this.Controls.Add( this.btnEditAccount );
            this.Controls.Add( this.btnCancelRegistration );
            this.Controls.Add( this.btnActivateAccount );
            this.Controls.Add(this.btnActivateShortAccount);
            this.Controls.Add(this.btnConvertToShortPrereg);
            this.Controls.Add( this.panel );
            this.Controls.Add( this.contextLabel );
            this.Name = "WorklistsView";
            this.Size = new System.Drawing.Size( 1024, 619 );
            this.Load += new System.EventHandler( this.WorklistsView_Load );
            this.panel.ResumeLayout( false );
            this.tabAccounts.ResumeLayout( false );
            this.tabPreRegistration.ResumeLayout( false );
            this.tabPostRegistration.ResumeLayout( false );
            this.tabInsurance.ResumeLayout( false );
            this.tabLiability.ResumeLayout( false );
            this.tabPreMSE.ResumeLayout( false );
            this.tabNoShow.ResumeLayout( false );
            this.ResumeLayout( false );
        }
        #endregion

        #endregion

        #region Construction and Finalization

        public WorklistsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );

            tabAccounts.SelectedIndex = -1;

            postRegWorklistView.WorklistsView = this;
            insuranceWorklistView.WorklistsView = this;
            liabilityWorklistView.WorklistsView = this;
            preMSEWorklistView.WorklistsView = this;
            noShowWorklistView.WorklistsView = this;
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
        private static readonly ILog c_log = LogManager.GetLogger( typeof( WorklistsView ) );

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private enum ScreenIndexes { PREREGISTRATION, POSTREGISTRATION, INSURANCE, LIABILITY, PREMSE, NOSHOW };

        private AccountProxy accountProxy;
        private Worklist worklist;
        private ActionsList actionCollection;

        private ClickOnceLoggingButton btnActivateShortAccount;
        private ClickOnceLoggingButton btnActivateAccount;
        private ClickOnceLoggingButton btnCancelRegistration;
        private ClickOnceLoggingButton btnEditAccount;
        private ClickOnceLoggingButton btnConvertToShortPrereg;

        private ColumnHeader columnHdrAction;
        private ColumnHeader columnHdrCount;

        private Label lblToDoListFor;
        private Label lblAccountName;

        private ListView listView;

        private Panel panel;

        private TabControl tabAccounts;

        private TabPage tabPreRegistration;
        private TabPage tabPostRegistration;
        private TabPage tabInsurance;
        private TabPage tabLiability;
        private TabPage tabPreMSE;
        private TabPage tabNoShow;

        private UserContextView contextLabel;

        private PostRegWorklistView postRegWorklistView;
        private PreRegWorklistView preRegWorklistView;
        private InsuranceWorklistView insuranceWorklistView;
        private LiabilityWorklistView liabilityWorklistView;
        private PreMSEWorklistView preMSEWorklistView;
        private NoShowWorklistView noShowWorklistView;
        private CancelPreRegistrationView cancelPreRegistrationView;

        private AccountProxy i_SelectedAccountProxy;
        private bool actionItemsToDisplay;

        private ScreenIndexes selectedTab;

        #endregion

        #region Constants
        private const string NOT_PRE_MSE_ACCOUNT_MSG = "The requested action cannot proceed because the account is no longer a Pre-MSE ED account.";
        public Label lblItems;
        private const string ACCOUNT_CANCELED_MSG = "The requested action cannot proceed because the account has been canceled.";
        #endregion
    }
}