using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using Extensions.Exceptions;
using Extensions.SecurityService.Domain;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.AnnouncementViews;
using PatientAccess.UI.CensusInquiries;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DischargeViews;
using PatientAccess.UI.DocumentImagingViews;
using PatientAccess.UI.ExceptionManagement;
using PatientAccess.UI.FUSNotes;
using PatientAccess.UI.HelpViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.HistoricalAccountViews;
using PatientAccess.UI.Logging;
using PatientAccess.UI.NewEmployersManagement;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PreMSEViews;
using PatientAccess.UI.PreRegistrationViews;
using PatientAccess.UI.QuickAccountCreation.ViewImpl;
using PatientAccess.UI.Registration;
using PatientAccess.UI.Reports;
using PatientAccess.UI.ShortRegistration;
using PatientAccess.UI.TransferViews;
using PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient.ViewImpl;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.UI.UCCPreMSEViews;
using PatientAccess.UI.WorklistViews;
using log4net;
using Facility = Peradigm.Framework.Domain.Parties.Facility;
using Timer = System.Timers.Timer;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI
{
    /// <summary>
    /// Summary description for PatientAccessView.
    /// </summary>
    [Serializable]
    public class PatientAccessView : TimeOutFormView
    {
        
        #region Event Handlers

        private void ActionSelectedEventHandler(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            Cursor storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                if (args.Context == null)
                {
                    ClearPanel();
                }
                else
                {
                    AccountView accountView = sender as AccountView;

                    if (Panel.Controls.Contains(worklistsView))
                    {
                        maintenanceCmdView = MaintenanceCmdView.GetInstance();
                        if (Panel.Controls.IndexOf(maintenanceCmdView) == -1)
                            Panel.Controls.Add(maintenanceCmdView);

                        int index = Panel.Controls.IndexOf(maintenanceCmdView);
                        if (index != -1)
                        {
                            maintenanceCmdView.AccountView = accountView;
                            maintenanceCmdView.ReturnToMainScreen += Cancel_Click;

                            Panel.Controls[index].Show();
                            Panel.Controls[index].BringToFront();
                            Panel.Controls[index].Invalidate(true);
                            Panel.Controls[index].Update();
                            Invalidate(true);
                            Update();
                            Focus();
                        }

                    }
                }
            }
            finally
            {
                Cursor = storedCursor;
            }
        }

        /// <summary>
        /// Handles the Load event of the <see cref="PatientAccessView"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PatientAccessView_Load(object sender, EventArgs e)
        {
            Invalidate(true);
            ClearPanel();

            DisplayAnnouncementView();

            DisplayStatusBarItems();

            // SR 41094 - January 2008 Release
            // Initialize icons/menu options for previously scanned documents

            SetPreviousDocumentOptions();
            SetFUSNotesOptions(false);

            SetUpMenu(mainMenu);

            // Precache Reference Data
            User patientAccessUser = User.GetCurrent();

            new ReferenceObjectsLoader(patientAccessUser.Facility.Oid).LoadAllAsync();
        }

        private void SetUpMenu(MainMenu menu)
        {
            // set the text for the menu options based on the corresponding activity

            mnuRegisterOnlinePreRegistration.Text = new OnlinePreRegistrationActivity().Description;
            mnuRegisterPreRegisterPatient.Text = new PreRegistrationActivity().Description;
            mnuRegisterRegisterPatient.Text = new RegistrationActivity().Description;
            mnuRegisterShortPreRegisterPatient.Text = new ShortPreRegistrationActivity().Description;
            mnuRegisterShortRegisterPatient.Text = new ShortRegistrationActivity().Description;
            menuRegisterPatientPreMse.Text = new PreMSERegisterActivity().Description;
            menuRegisterPatientPostMse.Text = new PostMSERegistrationActivity().Description;
            mnuRegisterPreRegisterNewborn.Text = new PreAdmitNewbornActivity().Description;
            mnuRegisterRegisterNewborn.Text = new AdmitNewbornActivity().Description;
            menuRegisterPatientUCCPreMse.Text = new UCCPreMSERegistrationActivity().Description;
            menuRegisterPatientUCCPostMse.Text = new UCCPostMseRegistrationActivity().Description;
            mnuRegisterEditAccount.Text = new EditAccountActivity().Description;
            mnuPrintFaceSheet.Text = new PrintFaceSheetActivity().Description;
            mnuRegisterViewAccount.Text = new ViewAccountActivity().Description;
            mnuRegisterCancelPreregistration.Text = new CancelPreRegActivity().Description;
            mnuRegisterCancelInpatientStatus.Text = new CancelInpatientStatusActivity().Description;
            mnuRegisterPreRegistrationOffline.Text = new PreRegistrationWithOfflineActivity().Description;
            mnuRegisterRegistrationOffline.Text = new RegistrationWithOfflineActivity().Description;

            mnuRegisterShortPreRegistrationOffline.Text = new ShortPreRegistrationWithOfflineActivity().Description;
            mnuRegisterShortRegistrationOffline.Text = new ShortRegistrationWithOfflineActivity().Description;

            mnuRegisterRegisterPreMSEOffline.Text = new PreMSERegistrationWithOfflineActivity().Description;
            mnuRegisterRegisterNewbornOffline.Text = new AdmitNewbornWithOfflineActivity().Description;
            mnuQuickAccountCreation.Text = new QuickAccountCreationActivity().Description;
            mnuPAIWalkInOutpatientAccountCreation.Text = new PAIWalkinOutpatientCreationActivity().Description ;
            mnuRegisterPreRegisterNewbornOffline.Text = new PreAdmitNewbornWithOfflineActivity().Description;


            mnuDischargePreDischarge.Text = new PreDischargeActivity().Description;
            mnuDischargeDischargePatient.Text = new DischargeActivity().Description;
            mnuDischargeEditDischarge.Text = new EditDischargeDataActivity().Description;
            mnuEditRecurringOutpatient.Text = new EditRecurringDischargeActivity().Description;
            mnuDischargeCancelDischarge.Text = new CancelInpatientDischargeActivity().Description;
            mnuDischargeCancelOutpatientDischarge.Text = new CancelOutpatientDischargeActivity().Description;

            mnuTransferPatientLocation.Text = new TransferActivity().Description;
            mnuTransferOutPatToInPat.Text = new TransferOutToInActivity().Description;
            mnuTransferInPatToOutPat.Text = new TransferInToOutActivity().Description;
            mnuTransferERPatientToOutpatient.Text = new TransferERToOutpatientActivity().Description;
            mnuTransferOutpatientToERPatient.Text = new TransferOutpatientToERActivity().Description;
            mnuTransferSwapPatientLocations.Text = new TransferBedSwapActivity().Description;

            mnuWorklistsPreRegWorklist.Text = new PreRegistrationWorklistActivity().Description;
            mnuWorklistsPostRegWorklist.Text = new PostRegistrationWorklistActivity().Description;
            mnuWorklistsInsuranceWorklist.Text = new InsuranceVerificationWorklistActivity().Description;
            mnuWorklistsLiabilityWorklist.Text = new PatientLiabilityWorklistActivity().Description;
            mnuWorklistsPreMseWorklist.Text = new PreMSEWorklistActivity().Description;
            mnuWorklistsNoShowWorklist.Text = new NoShowWorklistActivity().Description;

            mnuCensusByPatient.Text = new CensusByPatientActivity().Description;
            mnuCensusbyNursingStation.Text = new CensusByNursingStationActivity().Description;
            mnuCensusbyADT.Text = new CensusByADTActivity().Description;
            mnuCensusbyPhysician.Text = new CensusByPhysicianActivity().Description;
            mnuCensusbyBloodless.Text = new CensusByBloodlessActivity().Description;
            mnuCensusbyReligion.Text = new CensusByReligionActivity().Description;
            mnuCensusbyPayorBroker.Text = new CensusByPayorActivity().Description;

            mnuReportsPhysicians.Text = new PhysiciansReportActivity().Description;

            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
            Facility securityFrameworkFacility = new Facility(patientAccessUser.Facility.Code, patientAccessUser.Facility.Description);

            Menu.MenuItemCollection menuCollection = menu.MenuItems;

            foreach (MenuItem item in menuCollection)
            {
                Menu.MenuItemCollection menuSubCollection = item.MenuItems;
                foreach (MenuItem subItem in menuSubCollection)
                {
                    string subItemText = subItem.Text;
                    bool hasAddPermissionToView = securityUser.HasPermissionTo(Privilege.Actions.View, subItemText, securityFrameworkFacility);
                    if (!(hasAddPermissionToView))
                    {
                        subItem.Enabled = false;
                    }
                }
                DisableUCCRegistration();
            }
        }

        private void DisableUCCRegistration()
        {
            if ( !User.GetCurrent().Facility.IsUCCRegistrationEnabled )
            {
                menuRegisterPatientUCCPreMse.Enabled = false;
                menuRegisterPatientUCCPostMse.Enabled = false;
            }
        }

        private void PatientAccess_Resize(object sender, EventArgs e)
        {
            if (minClientSize == Size.Empty)
            {   // Set the minimum size of the frame
                Rectangle rectangle = Bounds;
                minClientSize = new Size(rectangle.Width, rectangle.Height);
                MinimumSize = minClientSize;
            }

            Invalidate();
            Update();
        }

        private void On_PatientAccessViewPaint(object sender, PaintEventArgs e)
        {
            mainToolBar.Invalidate(true);
            Rectangle rectangle = new Rectangle(0, 0, Width, Height);
            LinearGradientBrush brush = new LinearGradientBrush(
                rectangle,
                Color.FromArgb(195, 218, 249),
                Color.FromArgb(138, 177, 242),
                LinearGradientMode.Horizontal
                );
            e.Graphics.FillRectangle(brush, rectangle);
            mainToolBar.Invalidate(true);
            mainToolBar.Update();
        }

        // File Menu Event Handlers
        //
        private void mnuFileLogOff_Click(object sender, EventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log("File | LogOff menu selected");
            SetAccountViewType( false, false, false);
            DialogResult = DialogResult.Yes;
        }

        private void mnuFileLogOffExit_Click(object sender, EventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log("File | LogOff | Exit menu selected");
            SetAccountViewType( false, false, false);
            DialogResult = DialogResult.OK;
        }

        private void PatientAccessView_Closing(object sender, CancelEventArgs e)
        {
            if (ConfirmedMenuClick(true))
            {
                try
                {
                    ActivityEventAggregator.GetInstance().RemoveAllListeners();
                    SearchEventAggregator.GetInstance().RemoveAllListeners();
                    PatientAccessViewPopulationAggregator.GetInstance().RemoveAllListeners();
                    WorklistCmdAggregator.GetInstance().RemoveAllListeners();
                    CensusEventAggregator.GetInstance().RemoveAllListeners();
                    OKTAServiceFeatureManager oKTAServiceFeatureManager = new OKTAServiceFeatureManager();
                    User patientAccessUser = User.GetCurrent();
                    bool IsOKTAEnabled = oKTAServiceFeatureManager.IsOKTAEnabled(patientAccessUser.Facility);
                    if (!IsOKTAEnabled)
                    {
                        //we get here right after mnuFileLogOff_Click and mnuFileLogOffExit_Click, and if User closed the App using "X" in upper right corner
                        //fyi: when User closes the App using "X" in upper right corner then DialogResult := DialogResult.Cancel by default
                        IUserBroker userBroker = BrokerFactory.BrokerOfType<IUserBroker>();
                        userBroker.Logout(User.GetCurrent());
                    }

                    // The WebBrowser control hangs on to session information if the application keeps
                    // running. This will ensure that it is cleared.
                    ExtendedWebBrowser.ClearSession();

                }
                catch (Exception ex)
                {
                    // OTD 17426 - write to log file on user's machine without showing exception dialog 
                    BreadCrumbLogger.GetInstance.Log(String.Format("Fail to log out: {0}", ex));
                }
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        private void mnuRegisterPreRegisterPatient_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | PreRegisterPatient menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                PreRegistrationView preRegistrationView = new PreRegistrationView();
                preRegistrationView.MPIV.CurrentActivity = preRegistrationView.CurrentActivity;
                CurrentActivity = new PreRegistrationActivity();
                preRegistrationView.ReturnToMainScreen += Cancel_Click;
              
                preRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(preRegistrationView);
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ClearPanel();
            SetAccountViewType( false, false, false);
            DisplayAnnouncementView();
        }

        private void mnuRegisterRegisterPatient_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | RegisterPatient menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                RegistrationView registrationView = new RegistrationView {CurrentActivity = new RegistrationActivity()};
                registrationView.MPIV.CurrentActivity = registrationView.CurrentActivity;
                CurrentActivity = new RegistrationActivity();
                registrationView.ReturnToMainScreen += Cancel_Click;
              
                registrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(registrationView);
            }
        }

        private void mnuRegisterShortRegisterPatient_Click( object sender, EventArgs e )
        {
            if ( ConfirmedMenuClick() )
            {
                BreadCrumbLogger.GetInstance.Log( "Register | Register Diagnostic Outpatient menu selected" );
                ClearPanel();
                SetAccountViewType( true, false, false);

                ShortRegistrationView shortRegistrationView = new ShortRegistrationView
                { CurrentActivity = new ShortRegistrationActivity() };
                shortRegistrationView.MPIV.CurrentActivity = shortRegistrationView.CurrentActivity;
                CurrentActivity = new ShortRegistrationActivity();
                shortRegistrationView.ReturnToMainScreen += Cancel_Click;
              
                shortRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add( shortRegistrationView );
            }
        }

        private void mnuRegisterShortPreRegisterPatient_Click( object sender, EventArgs e )
        {
            if ( ConfirmedMenuClick() )
            {
                BreadCrumbLogger.GetInstance.Log( "Register | PreRegister Diagnostic Outpatient menu selected" );
                ClearPanel();
                SetAccountViewType( true, false, false);
                
                ShortPreRegistrationView preRegistrationView = new ShortPreRegistrationView
                { CurrentActivity = new ShortPreRegistrationActivity() };
                preRegistrationView.MPIV.CurrentActivity = preRegistrationView.CurrentActivity;
                CurrentActivity = new ShortPreRegistrationActivity();
                preRegistrationView.ReturnToMainScreen += Cancel_Click;
                
                preRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add( preRegistrationView );
            }
        }

        private void menuRegisterPatientPreMse_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | PatientPreMse menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // we instantiate both the PreMSE and PreReg views because we don't know
                // if the user will choose to edit a PT=3, FC=37 (PreMSE) or other account
                // which view is displayed is determined once the account is selected...

                PreMseSearchView preMseRegistrationView = new PreMseSearchView();
                CurrentActivity = new PreMSERegisterActivity();
                preMseRegistrationView.ReturnToMainScreen += Cancel_Click;
                
                preMseRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(preMseRegistrationView);
            }
        }

        private void menuRegisterPatientPostMse_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | PatientPostMse menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                PostMSERegistrationView postMSERegistrationView = new PostMSERegistrationView
                    { CurrentActivity = new PostMSERegistrationActivity() };

                postMSERegistrationView.MPIV.CurrentActivity = postMSERegistrationView.CurrentActivity;
                CurrentActivity = new PostMSERegistrationActivity();
                postMSERegistrationView.ReturnToMainScreen += Cancel_Click;
                
                postMSERegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(postMSERegistrationView);
            }
        }

        private void menuRegisterPatientUCCPreMse_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | Patient UCCPreMse menu selected");
                ClearPanel();
                SetAccountViewType(false, false, false); 
           
                UCCPreMseSearchView UCCPreMseRegistrationView = new UCCPreMseSearchView();
                CurrentActivity = new UCCPreMSERegistrationActivity();
                UCCPreMseRegistrationView.ReturnToMainScreen += Cancel_Click;

                UCCPreMseRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(UCCPreMseRegistrationView);
            }
        }

        private void menuRegisterPatientUCCPostMse_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | PatientUCCPostMse menu selected");
                ClearPanel();
                SetAccountViewType(false, false, false);

                PostMSERegistrationView UCCPostMSERegistrationView = new PostMSERegistrationView { CurrentActivity = new UCCPostMseRegistrationActivity() };

                UCCPostMSERegistrationView.MPIV.CurrentActivity = UCCPostMSERegistrationView.CurrentActivity;
                CurrentActivity = new UCCPostMseRegistrationActivity();
                UCCPostMSERegistrationView.ReturnToMainScreen += Cancel_Click;

                UCCPostMSERegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(UCCPostMSERegistrationView);
            }
        }
        private void mnuRegisterPreRegisterNewborn_Click( object sender, EventArgs e )
        {
            if ( ConfirmedMenuClick() )
            {
                BreadCrumbLogger.GetInstance.Log( "Register | PreRegisterNewborn menu selected" );
                ClearPanel();
                SetAccountViewType( false, false, false);

                NewbornPreRegistrationView newBornPreRegistrationView = new NewbornPreRegistrationView();
                CurrentActivity = new PreAdmitNewbornActivity();
                newBornPreRegistrationView.ReturnToMainScreen += Cancel_Click;
                
                newBornPreRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add( newBornPreRegistrationView );
            }
        }

        private void mnuRegisterRegisterNewborn_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | RegisterNewborn menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                NewBornRegistrationView newBornRegistrationView = new NewBornRegistrationView();
                CurrentActivity = new AdmitNewbornActivity();
                newBornRegistrationView.ReturnToMainScreen += Cancel_Click;
       
                newBornRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(newBornRegistrationView);
            }
        }

        private void mnuRegisterEditAccount_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | EditAccount menu selected");

                // default to a PreReg view... we don't know at the time that the user selects the menu.
                // Regardless, we update the Activity on the Edit/Maintain click event...
                ClearPanel();
                SetAccountViewType( false, false, false);
                PreRegistrationView preRegistrationView = new PreRegistrationView
                    {CurrentActivity = new MaintenanceActivity()};
                CurrentActivity = new MaintenanceActivity();
                preRegistrationView.MPIV.CurrentActivity = preRegistrationView.CurrentActivity;
                preRegistrationView.ReturnToMainScreen += Cancel_Click;
 
                preRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(preRegistrationView);
            }
        }

        private void mnuPrintFaceSheet_Click(object sender, EventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log("Register | PrintFaceSheet menu selected");
           

            HistoricalPatientSearch historicalPatientSearch = new HistoricalPatientSearch
                { CurrentActivity = new PrintFaceSheetActivity() };
            CurrentActivity = new PrintFaceSheetActivity();

            try
            {
                historicalPatientSearch.ShowDialog(this);
            }
            finally
            {
                historicalPatientSearch.Dispose();
            }
        }

        private void mnuRegisterViewAccount_Click(object sender, EventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log("Register | ViewAccount menu selected");
            
            HistoricalPatientSearch historicalPatientSearch = new HistoricalPatientSearch
                {CurrentActivity = new ViewAccountActivity()};
            CurrentActivity = new ViewAccountActivity();

            try
            {
                historicalPatientSearch.ShowDialog(this);
            }
            finally
            {
                historicalPatientSearch.Dispose();
            }
        }

        private void mnuRegisterCancelPreregistration_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | CancelPreregistration menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                CurrentActivity = new CancelPreRegActivity();
                var preRegistrationSearchView = new PreRegistrationSearchView(new CancelPreRegActivity()) {Dock = DockStyle.Fill};
                Panel.Controls.Add(preRegistrationSearchView);
            }
        }

        private void mnuRegisterCancelInpatientStatus_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | CancelInpatientStatus menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                CurrentActivity = new CancelInpatientStatusActivity();
                dischargeView = 
                    new DischargeView(new CancelInpatientStatusActivity()) {Dock = DockStyle.Fill};

                Panel.Controls.Add(dischargeView);
            }
        }

        // Offline entry

        private void mnuRegisterPreRegistrationOffline_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | PreRegistrationOffline menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                PreRegistrationView preRegistrationView = new PreRegistrationView
                    { CurrentActivity = { AssociatedActivityType = typeof (PreRegistrationWithOfflineActivity ) } };
                CurrentActivity = new PreRegistrationActivity();
                preRegistrationView.MPIV.CurrentActivity = preRegistrationView.CurrentActivity;
                preRegistrationView.ReturnToMainScreen += Cancel_Click;
   
                preRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(preRegistrationView);
            }
        }

        private void mnuRegisterRegistrationOffline_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | RegistrationOffline menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                RegistrationView registrationView = new RegistrationView
                    { CurrentActivity = new RegistrationActivity { AssociatedActivityType = typeof ( RegistrationWithOfflineActivity ) } };
                CurrentActivity = new RegistrationActivity();
                registrationView.MPIV.CurrentActivity = registrationView.CurrentActivity;
                registrationView.ReturnToMainScreen += Cancel_Click;
              
                registrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(registrationView);
            }
        }

        // Offline entry

        private void mnuRegisterShortPreRegistrationOffline_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | Short PreRegistrationOffline menu selected");
                ClearPanel();
                SetAccountViewType(true, false, false);
                
                ShortPreRegistrationView shortPreRegistrationView = new ShortPreRegistrationView { CurrentActivity = new ShortPreRegistrationActivity { AssociatedActivityType = typeof(ShortPreRegistrationWithOfflineActivity) } };
                CurrentActivity = new ShortPreRegistrationActivity();
                shortPreRegistrationView.MPIV.CurrentActivity = shortPreRegistrationView.CurrentActivity;
                shortPreRegistrationView.ReturnToMainScreen += Cancel_Click;

                shortPreRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(shortPreRegistrationView);
            }
        }

        private void mnuRegisterShortRegistrationOffline_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | Short RegistrationOffline menu selected");
                ClearPanel();
                SetAccountViewType( true, false, false);
            
                ShortRegistrationView shortRegistrationView = new ShortRegistrationView { CurrentActivity = new ShortRegistrationActivity { AssociatedActivityType = typeof(ShortRegistrationWithOfflineActivity) } };
                CurrentActivity = new ShortRegistrationActivity();
                shortRegistrationView.MPIV.CurrentActivity = shortRegistrationView.CurrentActivity;
                shortRegistrationView.ReturnToMainScreen += Cancel_Click;

                shortRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(shortRegistrationView);
            }
        }
        private void mnuRegisterRegisterPreMSEOffline_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | RegisterPreMSEOffline menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                PreMseSearchView preMseRegistrationView = new PreMseSearchView();
                CurrentActivity = new PreMSERegisterActivity();
                preMseRegistrationView.CurrentActivity.AssociatedActivityType = typeof(PreMSERegistrationWithOfflineActivity);
                preMseRegistrationView.ReturnToMainScreen += Cancel_Click;
         
                preMseRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(preMseRegistrationView);
            }
        }

        private void mnuRegisterRegisterNewbornOffline_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | RegisterNewbornOffline menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                CurrentActivity = new AdmitNewbornActivity();
                NewBornRegistrationView newBornRegistrationView = new NewBornRegistrationView();
                newBornRegistrationView.CurrentActivity.AssociatedActivityType = typeof(AdmitNewbornWithOfflineActivity);
                newBornRegistrationView.ReturnToMainScreen += Cancel_Click;
        
                newBornRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(newBornRegistrationView);
            }
        }

        private void mnuRegisterPreRegisterNewbornOffline_Click( object sender, EventArgs e )
        {
            if ( ConfirmedMenuClick() )
            {
                BreadCrumbLogger.GetInstance.Log( "Register | PreRegisterNewbornOffline menu selected" );
                ClearPanel();
                SetAccountViewType( false, false, false);

                CurrentActivity = new PreAdmitNewbornActivity();
                NewbornPreRegistrationView newbornPreRegistrationView = new NewbornPreRegistrationView();
                newbornPreRegistrationView.CurrentActivity.AssociatedActivityType = typeof( PreAdmitNewbornWithOfflineActivity );
                newbornPreRegistrationView.ReturnToMainScreen += Cancel_Click;
          
                newbornPreRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add( newbornPreRegistrationView );
            }
        }

        //  Discharge Menu Event Handlers
        //
        private void mnuDischargePreDischarge_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Discharge | PreDischarge menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                dischargeView = new DischargeView(new PreDischargeActivity());
                CurrentActivity = new PreDischargeActivity();

                dischargeView.Dock = DockStyle.Fill;
                Panel.Controls.Add(dischargeView);

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuDischargePatient_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Discharge | DischargePatient menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                dischargeView = new DischargeView(new DischargeActivity());
                CurrentActivity = new DischargeActivity();

                dischargeView.Dock = DockStyle.Fill;
                Panel.Controls.Add(dischargeView);

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuEditDischarge_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Discharge | Edit Inpatient Discharge menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                dischargeView = new DischargeView(new EditDischargeDataActivity());
                CurrentActivity = new EditDischargeDataActivity();

                dischargeView.Dock = DockStyle.Fill;
                Panel.Controls.Add(dischargeView);

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuEditRecurringOutpatient_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Discharge | EditRecurringOutpatient discharge menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                dischargeView = new DischargeView(new EditRecurringDischargeActivity());
                CurrentActivity = new EditRecurringDischargeActivity();

                dischargeView.Dock = DockStyle.Fill;
                Panel.Controls.Add(dischargeView);

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuCancelDischarge_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Discharge | Cancel Inpatient discharge menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                dischargeView = new DischargeView(new CancelInpatientDischargeActivity());

                CurrentActivity = new CancelInpatientDischargeActivity();
                dischargeView.Dock = DockStyle.Fill;
                Panel.Controls.Add(dischargeView);

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuCancelOutpatientDischarge_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Discharge | Cancel Outpatient discharge menu selected");
                ClearPanel();
                SetAccountViewType( false, false, false);

                // next line is temporary; once the dischargeView has been broken into each of its
                // controllers, an empty constructor should be called (and the CurrentActivity property on the
                // appropriate controller changed to a 'fixed' activity.

                dischargeView = new DischargeView(new CancelOutpatientDischargeActivity());

                CurrentActivity = new CancelOutpatientDischargeActivity();
                dischargeView.Dock = DockStyle.Fill;
                Panel.Controls.Add(dischargeView);

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        //  Transfer Menu Event Handlers
        //
        private void mnuTransferPatientLocation_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                // TLG 10/3/2005 May need refactored

                BreadCrumbLogger.GetInstance.Log("Transfer | TransferPatientLocation menu selected");
                SetAccountViewType( false, false, false);
                CurrentActivity = new TransferActivity();

                DoPatientTransfer(sender, e, new TransferActivity());

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuTransferInPatToOutPat_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Transfer | TransferInPatToOutPat menu selected");
                SetAccountViewType( false, false, false);

                // TLG 10/3/2005 May need refactored
                CurrentActivity = new TransferInToOutActivity();
                DoPatientTransfer(sender, e, new TransferInToOutActivity());

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuTransferERToOutpatientPatient_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Transfer | TransferERPatToOutPat menu selected");
                SetAccountViewType(false, false, false);
                
                CurrentActivity = new TransferERToOutpatientActivity();
                DoPatientTransferERPatToOutpatType(sender, e, new TransferERToOutpatientActivity());

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }
        private void mnuTransferOutpatientToERPatient_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Transfer | TransferOutPatToERPat menu selected");
                SetAccountViewType(false, false, false);
                 
                CurrentActivity = new TransferOutpatientToERActivity();
                DoPatientTransferOutPatientToERPatType(sender, e, new TransferOutpatientToERActivity());

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuTransferOutPatToInPat_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Transfer | TransferOutPatToInPat menu selected");
                SetAccountViewType( false, false, false);
                CurrentActivity = new TransferOutToInActivity();
                DoPatientTransfer(sender, e, new TransferOutToInActivity());

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuTransferSwapPatientLocations_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Transfer | TransferSwapPatientLocations menu selected");
                SetAccountViewType( false, false, false);
                // TLG 10/3/2005 May need refactored
                CurrentActivity = new TransferBedSwapActivity();
                DoPatientTransfer(sender, e, new TransferBedSwapActivity());

                if (maintenanceCmdView != null)
                {
                    maintenanceCmdView.RemoveAccountSelectedHandler();
                }
            }
        }

        private void mnuRegisterOnlinePreRegistration_Click( object sender, EventArgs e )
        {
            if ( ConfirmedMenuClick() )
            {
                BreadCrumbLogger.GetInstance.Log( "Register | View Online PreRegistration Submissions" );
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                string refreshInterval = ConfigurationManager.AppSettings[ApplicationConfigurationKeys.OFFLINELOCK_REFRESH_INTERVAL];
                var lockRefreshInterval = TimeSpan.Parse( refreshInterval );

                var onlinePreRegistrationView = new OnlinePreRegistrationSubmissionsView { Dock = DockStyle.Fill };
                CurrentActivity = new PreRegistrationActivity
                {
                    AssociatedActivity = new OnlinePreRegistrationActivity(lockRefreshInterval), 
                    AssociatedActivityType = typeof( OnlinePreRegistrationActivity )
                };

                Panel.Controls.Add( onlinePreRegistrationView );
                var preRegistrationSubmissionsBroker = BrokerFactory.BrokerOfType<IPreRegistrationSubmissionsBroker>();
                var onlinePreRegistrationSubmissionsPresenter = new OnlinePreRegistrationSubmissionsPresenter( 
                    User.GetCurrent().Facility, onlinePreRegistrationView, CurrentActivity, preRegistrationSubmissionsBroker );
                onlinePreRegistrationSubmissionsPresenter.UpdateView();
                Cursor = storedCursor;
            }
        }

        // Worklists Menu Event Handlers
        //
        private void mnuWorklistsPreRegWorklist_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Worklists | WorklistsPreRegWorklist menu selected");
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                CurrentActivity = new PreRegistrationWorklistActivity();

                worklistsView = new WorklistsView {Dock = DockStyle.Fill};
                worklistsView.SetTabPage(0);
                Panel.Controls.Add(worklistsView);
                worklistsView.UpdateView();
                Cursor = storedCursor;
            }
        }

        private void mnuWorklistsPostRegWorklist_Click( object sender, EventArgs e )
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Worklists | worklistsPostRegWorklist menu selected");
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                CurrentActivity = new PostRegistrationWorklistActivity();

                worklistsView = new WorklistsView {Dock = DockStyle.Fill};
                worklistsView.SetTabPage(1);
                Panel.Controls.Add(worklistsView);
                worklistsView.UpdateView();
                Cursor = storedCursor;
            }
        }

        private void mnuWorklistsInsuranceWorklist_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Worklists | WorklistsInsuranceWorklist menu selected");
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                CurrentActivity = new InsuranceVerificationWorklistActivity();

                worklistsView = new WorklistsView {Dock = DockStyle.Fill};
                worklistsView.SetTabPage(2);
                Panel.Controls.Add(worklistsView);
                worklistsView.UpdateView();
                Cursor = storedCursor;
            }
        }

        private void mnuWorklistsLiabilityWorklist_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Worklists | WorklistsLiabilityWorklist menu selected");
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                CurrentActivity = new PatientLiabilityWorklistActivity();

                worklistsView = new WorklistsView {Dock = DockStyle.Fill};
                worklistsView.SetTabPage(3);
                Panel.Controls.Add(worklistsView);
                worklistsView.UpdateView();
                Cursor = storedCursor;
            }
        }

        private void mnuWorklistsPreMseWorklist_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Worklists | WorklistsPreMseWorklist menu selected");
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                CurrentActivity = new PreMSEWorklistActivity();

                worklistsView = new WorklistsView {Dock = DockStyle.Fill};
                worklistsView.SetTabPage(4);
                Panel.Controls.Add(worklistsView);
                worklistsView.UpdateView();
                Cursor = storedCursor;
            }
        }

        private void mnuWorklistsNoShowWorklist_Click(object sender, EventArgs e)
        {
            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Worklists | WorklistsNoShowWorklist menu selected");
                SetAccountViewType( false, false, false);

                Cursor storedCursor = Cursor;
                Cursor = Cursors.WaitCursor;

                ClearPanel();

                CurrentActivity = new NoShowWorklistActivity();

                // load, but hide, the registration view for the Activate event

                RegistrationView registrationView = new RegistrationView {Visible = false};
                registrationView.MPIV.CurrentActivity = registrationView.CurrentActivity;
                registrationView.ReturnToMainScreen += Cancel_Click;
                registrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(registrationView);

                worklistsView = new WorklistsView {Dock = DockStyle.Fill};
                worklistsView.SetTabPage(5);
                Panel.Controls.Add(worklistsView);
                worklistsView.UpdateView();
                Cursor = storedCursor;
            }
        }

        // Census Menu Event Handlers
        //
        private void mnuCensusByPatient_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByPatientActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyPatient menu selected");
            
            AssignSelectedCensus( CensusView.PATIENT );
        }

        private void mnuCensusbyNursingStation_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByNursingStationActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyNS menu selected");
             
            AssignSelectedCensus( CensusView.NURSINGSTATION );
        }

        private void mnuCensusbyADT_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByADTActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyADT menu selected");
           
            AssignSelectedCensus( CensusView.ADT );
        }

        private void mnuCensusbyPhysician_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByPhysicianActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyPhysician menu selected");
           
            AssignSelectedCensus( CensusView.PHYSICIAN );
        }

        private void mnuCensusbyBloodless_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByBloodlessActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyBloodless menu selected");
            SetAccountViewType( false, false, false);

            AssignSelectedCensus( CensusView.BLOODLESS );
        }

        private void mnuCensusbyReligion_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByReligionActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyRelegion menu selected");
        
            AssignSelectedCensus( CensusView.RELIGIONS );
        }

        private void mnuCensusbyPayorBroker_Click(object sender, EventArgs e)
        {
            CurrentActivity = new CensusByPayorActivity();
            BreadCrumbLogger.GetInstance.Log("Census | CensusbyPayorBroker menu selected");
          
            AssignSelectedCensus( CensusView.PAYOR );
        }

        // Reports Menu Event Handlers
        //
        private void mnuReportsPhysicians_Click(object sender, EventArgs e)
        {
            CurrentActivity = new PhysiciansReportActivity();
            BreadCrumbLogger.GetInstance.Log("Reports | Physicians menu selected");
        
            AssignInquiryString( FindaPhysicianView.PHYSICIANS_SEARCH );
        }

        // Help Menu Event Handlers
        private void mnuAboutPatientAccess_Click(object sender, EventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log("Help | About menu selected");
        
            AboutPatientAccessView aboutPatientAccessView = new AboutPatientAccessView();
            try
            {
                aboutPatientAccessView.ShowDialog(this);
            }
            finally
            {
                aboutPatientAccessView.Dispose();
            }
        }

        private void mnuPatientAccessHelp_Click(object sender, EventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log("Help | help menu selected");
            
            HelpService.DisplayHelp();
        }

        private void mnuNewEmployerManagement_Click( object sender, EventArgs e )
        {
            if ( !ConfirmedMenuClick() )
            {
                return;
            }

            BreadCrumbLogger.GetInstance.Log( "Register | Admin menu selected" );
            ClearPanel();
            SetAccountViewType( false, false, false);
            
            NewEmployerManagmentFacade newEmployerManagmentFacade = new NewEmployerManagmentFacade( this, ActivityEventAggregator.GetInstance() );

            newEmployerManagmentFacade.DisplayViewIfNotLocked();
        }
        private void SetAccountViewType(bool isShort, bool isQuick, bool isPAIWalkin)
        {
            AccountView.IsShortRegAccount = isShort;
            AccountView.IsQuickRegistered = isQuick;
            AccountView.IsPAIWalkinRegistered = isPAIWalkin;
        }
        protected virtual void PatientAccessView_ActivityStarted(object sender, EventArgs args)
        {
            IsActivityStarted = true;
        }

        private void PatientAccessView_ActivityCompleted(object sender, EventArgs e)
        {
            IsActivityStarted = false;
            LockedBedLocation = null;

            PatientAccessView_AccountUnLocked(sender, e);
            CancelBackgroundWorker();
            AccountView.GetInstance().StopBenefitsResponsePollTimer();
        }

        private void PatientAccessView_AccountLocked(object sender, EventArgs e)
        {

            LooseArgs args = (LooseArgs)e;
            IAccount iAccount = args.Context as IAccount;

            if ( iAccount == null )  return;

            if (LockedAccount == null || LockedAccount.AccountNumber == iAccount.AccountNumber)
            {
                LockedAccount = iAccount;
                StartAccountLockTimer(LockedAccount.Activity.Timeout.FirstAlertTime);
            }
            else if (LockedAccount2 == null || LockedAccount2.AccountNumber == iAccount.AccountNumber)
            {
                LockedAccount2 = iAccount;
            }

            if (LockedAccount != null && LockedAccount2 != null)
            {
                //for swappatientLocations account lock sync
                StopAccountLockTimer();

                StartAccountLockTimer(LockedAccount.Activity.Timeout.FirstAlertTime);
            }
        }

        private void PatientAccessView_AccountUnLocked(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            IAccount iAccount = args.Context as IAccount;

            StopAccountLockTimer();

            if (LockedAccount != null &&
                iAccount != null &&
                LockedAccount.AccountNumber == iAccount.AccountNumber)
            {
                LockedAccount = null;
                LockedAccount2 = null;
            }
        }

        private void PatientAccessView_BedLocked(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            LockedBedLocation = args.Context as Location;
        }

        internal void OnActivityCancelled(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                HandleActivityCancelled();
            }
            catch (Exception ex)
            {
                c_log.Error("OnActivityCancelled failed to release application resources. " + Environment.NewLine + ex.Message, ex);

                if (!sender.GetType().Equals(typeof(ApplicationExceptionHandler)))
                {
                    throw new EnterpriseException("OnActivityCancelled failed to release application resources.", ex, Severity.Catastrophic);
                }
            }

            CancelBackgroundWorker();
            IsActivityStarted = false;
            Cursor = Cursors.Default;

            StopBenefitsResponsePollTimer();
            GoHome();
        }

        protected virtual void HandleActivityCancelled()
        {
            AccountView.CloseVIweb();
            ForceRedrawOfScreen();

            // Release all locks incase of cancel activity only.
            ReleaseAccountLocks();
            ReleaseBedLock();
            UnloadRuleEngineHandlers();
        }

        private void StopBenefitsResponsePollTimer()
        {
            AccountView.GetInstance().StopBenefitsResponsePollTimer();
        }

        private void UnloadRuleEngineHandlers()
        {
            RuleEngine.GetInstance().UnloadHandlers();
        }

        private void ForceRedrawOfScreen()
        {
            Invalidate();
            Update();
        }

        private void PatientAccessView_ReturnToMainScreen(object sender, EventArgs e)
        {
            ClearPanel();
            DisplayAnnouncementView();
        }

        public void CloseAccountSupplementalInformationView()
        {
            DisableSupplementalInformationButton();

            if ( supplementalInformationView != null )
            {
                supplementalInformationView.AccountClosing = true;
                supplementalInformationView.Close();
            }
        }

        private void mnuHome_Click(object sender, EventArgs e)
        {
            SetAccountViewType( false, false, false);
            if ( ConfirmedMenuClick() )
            {
                GoHome();
            }
        }
        private void mainToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            // SR 41094 - January 2008 Release
            // Check if one of the previously scanned document icons or menu options were selected... if so,
            // invoke the handler for that document type.

            if (e.Button == btnADV)
            {
                ViewADV_Docs(null, null);
            }
            else if (e.Button == btnDL)
            {
                ViewDL_Docs(null, null);
            }
            else if (e.Button == btnINSCARD)
            {
                ViewINSCARD_Docs(null, null);
            }
            else if (e.Button == btnNPP)
            {
                ViewNPP_Docs(null, null);
            }
            else if (e.Button == btnFUSNotes)
            {
                ViewFUSNotes(null, null);
            }
            else if ( e.Button == btnViewOnlinePreRegSupplementalInformation )
            {
                DisableSupplementalInformationButton();

                supplementalInformationView = new SupplementalInformationView();
                var preRegistrationData = (( OnlinePreRegistrationActivity ) CurrentActivity.AssociatedActivity ).PreRegistrationData;
                var supplementalInformationPresenter = new SupplementalInformationPresenter( supplementalInformationView, preRegistrationData.SupplementalInformation );
                supplementalInformationPresenter.ShowInformationAsDialog();
            }
            else
            {
                controlWithFocus = null;

                SelectControlWithFocus(Panel, NOT_SELECT_ALL);

                TextBox textBox = (TextBox)controlWithFocus;
                if (textBox != null)
                {
                    if (e.Button == btnCut)
                    {
                        textBox.Cut();
                    }
                    else if (e.Button == btnCopy)
                    {
                        textBox.Copy();
                    }
                    else if (e.Button == btnPaste)
                    {
                        if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                        {
                            textBox.Paste();
                        }
                    }
                }
            }
        }
        private void CancelBackgroundWorker()
        {
            // cancel the background worker(s) here...
            if (null != backgroundWorker &&
                backgroundWorker.WorkerSupportsCancellation &&
                backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
        }

        public void EnableSupplementalInformationButton()
        {
            btnViewOnlinePreRegSupplementalInformation.Enabled = true;
        }

        private void DisableSupplementalInformationButton()
        {
            btnViewOnlinePreRegSupplementalInformation.Enabled = false;
        }

        /// <summary>
        /// DisablePreviousDocumentOptions - disable all the buttons for previous documents
        /// </summary>
        public void DisablePreviousDocumentOptions()
        {
            btnADV.Enabled = false;
            btnADV.ImageKey = IMAGE_ADVANCED_DIRECTIVE_DISABLED;

            btnDL.Enabled = false;
            btnDL.ImageKey = IMAGE_DRIVERS_LICENSE_DISABLED;

            btnINSCARD.Enabled = false;
            btnINSCARD.ImageKey = IMAGE_INSURANCE_CARD_DISABLED;

            btnNPP.Enabled = false;
            btnNPP.ImageKey = IMAGE_NPP_DISABLED;

            mnuViewADV.Enabled = false;
            mnuViewDL.Enabled = false;
            mnuViewINSCARD.Enabled = false;
            mnuViewNPP.Enabled = false;
        }

        /// <summary>
        /// DisablePreviousDocumentOption - disable a single previous document type button
        /// </summary>
        /// <param name="documentType"><see cref="VIWebPreviousDocument.VIWebPreviousDocumentType"/></param>
        private void DisablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType documentType)
        {
            if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.ADV)
            {
                btnADV.Enabled = false;
                btnADV.ImageKey = IMAGE_ADVANCED_DIRECTIVE_DISABLED;
                mnuViewADV.Enabled = false;
            }
            else if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.DL)
            {
                btnDL.Enabled = false;
                btnDL.ImageKey = IMAGE_DRIVERS_LICENSE_DISABLED;
                mnuViewDL.Enabled = false;
            }
            else if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD)
            {
                btnINSCARD.Enabled = false;
                btnINSCARD.ImageKey = IMAGE_INSURANCE_CARD_DISABLED;
                mnuViewINSCARD.Enabled = false;
            }
            else if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.NPP)
            {
                btnNPP.Enabled = false;
                btnNPP.ImageKey = IMAGE_NPP_DISABLED;
                mnuViewNPP.Enabled = false;
            }
        }


        /// <summary>
        /// EnablePreviousDocumentOption - enable a single previous document type button
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        private void EnablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType documentType)
        {
            if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.ADV)
            {
                btnADV.Enabled = true;
                btnADV.ImageKey = IMAGE_ADVANCED_DIRECTIVE_ENABLED;
                mnuViewADV.Enabled = true;
            }
            else if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.DL)
            {
                btnDL.Enabled = true;
                btnDL.ImageKey = IMAGE_DRIVERS_LICENSE_ENABLED;
                mnuViewDL.Enabled = true;
            }
            else if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD)
            {
                btnINSCARD.Enabled = true;
                btnINSCARD.ImageKey = IMAGE_INSURANCE_CARD_ENABLED;
                mnuViewINSCARD.Enabled = true;
            }
            else if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.NPP)
            {
                btnNPP.Enabled = true;
                btnNPP.ImageKey = IMAGE_NPP_ENABLED;
                mnuViewNPP.Enabled = true;
            }
        }

        /// <summary>
        /// GetPreviousDocuments - call the VI web service to return documents for the specified type.
        /// The criteria (number of docs to return, how far back to search, etc) are implemented in
        /// the broker.
        /// </summary>
        /// <param name="documentType">VIWebPreviousDocumentType</param>
        /// <returns>Collection of docs</returns>

        private VIWebPreviousDocuments GetPreviousDocuments(VIWebPreviousDocument.VIWebPreviousDocumentType documentType)
        {
            IVIWebServiceBroker iViWebServiceBroker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();

            Account account = Model as Account;
            if (account == null)
            {
                return new VIWebPreviousDocuments();
            }

            Insurance insurance = account.Insurance;
            Coverage coverage = insurance.PrimaryCoverage;
            InsurancePlan insurancePlan = coverage != null ? coverage.InsurancePlan : null;

            if (documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.UNKNOWN)
            {
                return iViWebServiceBroker.GetPreviousVisitDocuments(
                    account.Patient.Facility.Code,
                    account.Patient.MedicalRecordNumber,
                    insurancePlan);
            }
            
            return iViWebServiceBroker.GetPreviousVisitDocuments(
                account.Patient.Facility.Code,
                account.Patient.MedicalRecordNumber,
                insurancePlan,
                documentType);
        }


        /// <summary>
        /// View_Docs - view docs associated with an icon/menu option for the specified previous document type
        /// Pass the list of documents returned from the collection of documents returned by the web service call
        /// to the FormWebBrowserView for display
        /// </summary>
        /// <param name="documentType"><see cref="VIWebPreviousDocument.VIWebPreviousDocumentType"/></param>

        private void View_Docs(VIWebPreviousDocument.VIWebPreviousDocumentType documentType)
        {
            ArrayList documentIDs = new ArrayList();
            var vIwebHtml5Handler = new VIwebHTML5Handler();
            VIWebPreviousDocuments previousDocuments = GetPreviousDocuments(documentType);

            // build the string of documents to pass to the viewer

            if (previousDocuments.PreviousDocumentList.Count > 0)
            {
                foreach (VIWebPreviousDocument previousDocument in previousDocuments.PreviousDocumentList)
                {
                    documentIDs.Add(previousDocument.DocumentID);
                }

                VIwebHTML5Handler viwebhtml = new VIwebHTML5Handler();
                // If Facility enabled
                VIWEBFeatureManager = new VIWEBFeatureManager();
                bool IsFacilityEnabled = VIWEBFeatureManager.IsHTML5VIWebEnabledForFacility(Model as Account);
                if (IsFacilityEnabled)
                {
                    if (vIwebHtml5Handler.IsDynamsoftInstalled())
                    {
                        if (!vIwebHtml5Handler.IsChromeInstalled()
                            && !vIwebHtml5Handler.IsEdgeInstalled()
                            && !vIwebHtml5Handler.IsFirefoxInstalled())
                        {
                            MessageBox.Show(
                                UIErrorMessages.REQUIRED_MODERN_BROWSER_MSG,
                                UIErrorMessages.REQUIRED_MODERN_BROWSER_TITLE,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            LoadLegacyVIweb(documentIDs);
                        }
                        else
                        {
                            vIwebHtml5Handler.Model = Model as Account;
                            vIwebHtml5Handler.DoViewDocument(documentIDs);
                        }
                    }                        
                    else
                    {
                        MessageBox.Show(
                            UIErrorMessages.REQUIRED_DRIVER_TO_SCAN_MSG,
                            UIErrorMessages.REQUIRED_DRIVER_TO_SCAN_TITLE,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        LoadLegacyVIweb(documentIDs);
                    }
                }
                // If facility is not enabled for new VIweb
                else
                {
                    LoadLegacyVIweb(documentIDs);
                }

            }
        }

        public void LoadLegacyVIweb(ArrayList documentIDs)
        {
            FormWebBrowserView formWebBrowserView = new FormWebBrowserView();
            formWebBrowserView.Model = Model as Account;
            formWebBrowserView.UpdateView();
            formWebBrowserView.ViewDocument(documentIDs);
            formWebBrowserView.ShowDialog(this);
            formWebBrowserView.Dispose();            
        }

        /// <summary>
        /// ViewADV_Docs - view previously scanned Advanced Directive document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ViewADV_Docs(object sender, EventArgs e)
        {
            View_Docs(VIWebPreviousDocument.VIWebPreviousDocumentType.ADV);
        }

        /// <summary>
        /// ViewDL_Docs - view previously scanned Drivers License document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ViewDL_Docs(object sender, EventArgs e)
        {
            View_Docs(VIWebPreviousDocument.VIWebPreviousDocumentType.DL);
        }

        /// <summary>
        /// ViewINSCARD_Docs - view previously scanned Insurance Card documents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ViewINSCARD_Docs(object sender, EventArgs e)
        {
            View_Docs(VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
        }

        /// <summary>
        /// ViewNPP_Docs - view previously scanned Notice of Privacy Practice document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ViewNPP_Docs(object sender, EventArgs e)
        {
            View_Docs(VIWebPreviousDocument.VIWebPreviousDocumentType.NPP);
        }

        private void ViewFUSNotes(object sender, EventArgs e)
        {
            ViewFUSNotes viewFUSNotes = new ViewFUSNotes {Model_Account = Model as Account};
            viewFUSNotes.UpdateView();
            viewFUSNotes.ShowDialog(this);
        }

        private void mainToolBar_MouseLeave(object sender, EventArgs e)
        {
            imageRollOverTimer.Stop();
            btnCut.ImageIndex = 0;
            btnCopy.ImageIndex = 1;
            btnPaste.ImageIndex = 2;
        }

        /// <summary>
        /// Handles the MouseEnter event of the mainToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mainToolBar_MouseEnter(object sender, EventArgs e)
        {
            imageRollOverTimer.Interval = TIMESPAN;
            imageRollOverTimer.Start();
        }


        /// <summary>
        /// Handles the Click event of the mnuUndo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuUndo_Click(object sender, EventArgs e)
        {
            controlWithFocus = null;
            SelectControlWithFocus(Panel, NOT_SELECT_ALL);
            TextBox textBox = (TextBox)controlWithFocus;
            if (textBox != null)
            {
                if (textBox.CanUndo)
                {
                    textBox.Undo();
                    textBox.ClearUndo();
                }

            }

        }
        /// <summary>
        /// Handles the Click event of the mnuCut control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuCut_Click(object sender, EventArgs e)
        {
            controlWithFocus = null;
            SelectControlWithFocus(Panel, NOT_SELECT_ALL);
            TextBox textBox = (TextBox)controlWithFocus;
            if (textBox != null)
            {
                textBox.Cut();
            }
        }
        /// <summary>
        /// Handles the Click event of the mnuCopy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuCopy_Click(object sender, EventArgs e)
        {
            controlWithFocus = null;
            SelectControlWithFocus(Panel, NOT_SELECT_ALL);
            TextBox textBox = (TextBox)controlWithFocus;
            if (textBox != null)
            {
                textBox.Copy();
            }
        }
        /// <summary>
        /// Handles the Click event of the mnuPaste control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuPaste_Click(object sender, EventArgs e)
        {
            //check if any text is present in clipboard
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                controlWithFocus = null;
                SelectControlWithFocus(Panel, NOT_SELECT_ALL);
                TextBox textBox = (TextBox)controlWithFocus;
                if (textBox != null)
                {
                    textBox.Paste();
                }
            }
        }
        /// <summary>
        /// Handles the Click event of the mnuSelectAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mnuSelectAll_Click(object sender, EventArgs e)
        {
            controlWithFocus = null;
            SelectControlWithFocus(Panel, SELECT_ALL);
            TextBox textBox = (TextBox)controlWithFocus;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// SetPreviousDocumentOptions - accept an Account instance (where the Model for this view can not be relied upon) 
        /// and call SetPreviousDocumentOptions()
        /// </summary>
        /// <param name="anAccount">an Account instance</param>

        public void SetPreviousDocumentOptions(Account anAccount)
        {
            Account prevAccount = Model as Account;

            Model = anAccount;

            SetPreviousDocumentOptions();

            Model = prevAccount;
        }

        public void SetFUSNotesOptions(bool toggle)
        {
            // TLG 11/27/2007 - the FUS notes button will always be enabled to allow
            // the user to Add new notes :)
            btnFUSNotes.Enabled = toggle;
            mnuViewFUSNotes.Enabled = toggle;

            btnFUSNotes.ImageKey = toggle ? IMAGE_FUS_NOTES_ENABLED : IMAGE_FUS_NOTES_DISABLED;
        }

        private void DoGetPreviousVisitDocumentCnts(object sender, DoWorkEventArgs e)
        {
            IVIWebServiceBroker iViWebServiceBroker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();

            // poll CancellationPending and set e.Cancel to true and return 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (iViWebServiceBroker != null)
            {
                Account account = Model as Account;
                if (account == null)
                {
                    previousVisitDocumentCnts = new VIWebPreviousDocumentCnts();
                }
                else
                {
                    if (account.Insurance != null)
                    {
                        Insurance insurance = account.Insurance;
                        Coverage coverage = insurance.PrimaryCoverage;
                        InsurancePlan insurancePlan = coverage != null ? coverage.InsurancePlan : null;

                        previousVisitDocumentCnts = iViWebServiceBroker.GetPreviousVisitDocumentCnts(
                            account.Patient.Facility.Code,
                            account.Patient.MedicalRecordNumber,
                            insurancePlan);
                    }
                }
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }


        private void AfterGetPreviousVisitDocumentCnts(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed || Disposing)
                return;

            if (e.Cancelled)
            {

            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success

                if (previousVisitDocumentCnts.ADVDocumentCnt <= 0)
                {
                    DisablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.ADV);
                }
                else
                {
                    EnablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.ADV);
                }

                if (previousVisitDocumentCnts.DLDocumentCnt <= 0)
                {
                    DisablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.DL);
                }
                else
                {
                    EnablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.DL);
                }

                if ( Model != null && ( Model as Account ) != null && ( Model as Account ).Insurance != null )
                {
                    Coverage aCoverage = (Model as Account).Insurance.PrimaryCoverage;

                    if ((Model as Account).FinancialClass != null
                        && (aCoverage != null
                            && aCoverage.InsurancePlan != null
                            && aCoverage.InsurancePlan.PlanCategory != null)
                        && !( (Model as Account).Activity is PreMSERegisterActivity || (Model as Account).Activity is UCCPreMSERegistrationActivity))
                    {
                        if (previousVisitDocumentCnts.INSCardDocumentCnt <= 0)
                        {
                            DisablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
                        }
                        else
                        {
                            EnablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
                        }
                    }
                }
                else
                {
                    DisablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
                }

                if (previousVisitDocumentCnts.NPPDocumentCnt <= 0)
                {
                    DisablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.NPP);
                }
                else
                {
                    EnablePreviousDocumentOption(VIWebPreviousDocument.VIWebPreviousDocumentType.NPP);
                }
            }

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// SetPreviousDocumentOptions - set the icons/menu options based on the existence of previously scanned 
        /// documents associated with the current patient (based on Medical Record number). If a call to the VI web service
        /// returns documents, then the icon/option is enabled (with additional constraints for Insurance Cards - which require
        /// that a Primary coverage exists and that the activity is not PreMSE ).
        /// </summary>

        public void SetPreviousDocumentOptions()
        {
            if (Model == null ||
                ( (Model as Account) != null &&
                  (Model as Account).Patient == null) )
            {
                DisablePreviousDocumentOptions();
                return;
            }

            // get all the doc types
            if (backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoGetPreviousVisitDocumentCnts;
                backgroundWorker.RunWorkerCompleted += AfterGetPreviousVisitDocumentCnts;

                backgroundWorker.RunWorkerAsync();
            }
        }

        protected virtual void GoHome()
        {
            ClearPanel();
            CurrentActivity = null;
            DisplayAnnouncementView();
        }

        public void ActivateTab(string tabToActivate, IAccount iAccount)
        {
            SuspendLayout();

            ClearPanel();

            LooseArgs args = new LooseArgs(iAccount);
            //Account selectedAccount = iAccount.AsAccount();

            maintenanceCmdView = MaintenanceCmdView.GetInstance();
            maintenanceCmdView.ActivatingTab = tabToActivate;
            maintenanceCmdView.MasterPatientIndexView.Visible = false;
            maintenanceCmdView.Show();

            maintenanceCmdView.ReturnToMainScreen += Cancel_Click;

            maintenanceCmdView.Dock = DockStyle.Fill;
            Panel.Controls.Add(maintenanceCmdView);

            SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent(this, args);
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this, args);
            AccountView.GetInstance().WasInvokedFromWorklistItem = true;

            ResumeLayout();
        }

        /// <summary>
        /// Reloads the view.
        /// </summary>
        public void ReLoad()
        {
            Invalidate(true);
            ClearPanel();

            DisplayAnnouncementView();
        }

        public void LoadMaintenanceCmdView()
        {
            ClearPanel();

            MaintenanceCmdView cmdView = new MaintenanceCmdView();
            cmdView.ReturnToMainScreen += Cancel_Click;
            cmdView.Dock = DockStyle.Fill;
            Panel.Controls.Add(cmdView);
        }

        public void LoadEditPreMSEView()
        {
            ClearPanel();

            PreMseSearchView preMseRegistrationView = new PreMseSearchView();
            preMseRegistrationView.ReturnToMainScreen += Cancel_Click;
            preMseRegistrationView.Dock = DockStyle.Fill;
            Panel.Controls.Add(preMseRegistrationView);
        }
        public void LoadEditUCCPreMSEView()
        {
            ClearPanel();

            UCCPreMseSearchView preMseRegistrationView = new UCCPreMseSearchView();
            preMseRegistrationView.ReturnToMainScreen += Cancel_Click;
            preMseRegistrationView.Dock = DockStyle.Fill;
            Panel.Controls.Add(preMseRegistrationView);
        }

        public void LoadShortRegistrationView()
        {
            ClearPanel();
            SetAccountViewType( true, false, false);

            var empiPatient = CurrentActivity.EmpiPatient;
            ShortRegistrationView shortRegistrationView = new ShortRegistrationView { CurrentActivity = new ShortRegistrationActivity() };
            shortRegistrationView.MPIV.CurrentActivity = shortRegistrationView.CurrentActivity;
            CurrentActivity = new ShortRegistrationActivity();
            CurrentActivity.EmpiPatient = empiPatient;
            shortRegistrationView.ReturnToMainScreen += Cancel_Click;
            shortRegistrationView.Dock = DockStyle.Fill;
            Panel.Controls.Add( shortRegistrationView );
        }

        public void LoadRegistrationView()
        {
            ClearPanel();
            SetAccountViewType( false, false, false);

            var empiPatient = CurrentActivity.EmpiPatient;
            RegistrationView registrationView = new RegistrationView { CurrentActivity = new RegistrationActivity() };
            registrationView.MPIV.CurrentActivity = registrationView.CurrentActivity;
            CurrentActivity = new RegistrationActivity();
            CurrentActivity.EmpiPatient = empiPatient;
            registrationView.ReturnToMainScreen += Cancel_Click;
            registrationView.Dock = DockStyle.Fill;
            Panel.Controls.Add( registrationView );
        }

        public void LoadRegistrationView(Activity activity)
        {
            ClearPanel();
            SetAccountViewType( false, false, false);

            var empiPatient = CurrentActivity.EmpiPatient;
            RegistrationView registrationView = new RegistrationView { CurrentActivity = activity };
            registrationView.MPIV.CurrentActivity = registrationView.CurrentActivity;
            CurrentActivity = activity;
            CurrentActivity.EmpiPatient = empiPatient;
            registrationView.ReturnToMainScreen += Cancel_Click;
            registrationView.Dock = DockStyle.Fill;
            Panel.Controls.Add( registrationView );
        }

        private void DisableIconsAndMenuOptions()
        {
            // Disable all icons/menu options for FUS notes and Previously scanned documents
            SetFUSNotesOptions(false);
            DisablePreviousDocumentOptions();
        }
        #endregion

        #region Properties

        public Activity CurrentActivity
        {
            get
            {
                return i_CurrentActivity;
            }
            private set
            {
                i_CurrentActivity = value;
            }
        }

        public IAccount SelectedAccount
        {
            get
            {
                return i_SelectedAccount;
            }
            set
            {
                i_SelectedAccount = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Releases the bed lock.
        /// </summary>
        private void ReleaseBedLock()
        {
            if (LockedBedLocation != null)
            {
                AccountActivityService.ReleaseBedLock(LockedBedLocation);
                LockedBedLocation = null;
            }
        }

        /// <summary>
        /// Releases the account locks.
        /// </summary>
        private void ReleaseAccountLocks()
        {
            // OTD36407 - Swap Patient Location 'Cancel' is not releasing the second locked account.
            // Fix:- Moved this 'if' to the top since raising PatientAccessView_AccountUnLocked 
            // event before this was setting LockedAccount2 to null before releasing its lock. 
            if (LockedAccount2 != null)
            {
                AccountActivityService.ReleaseAccountlock(LockedAccount2);
            }

            if (LockedAccount != null)
            {
                AccountActivityService.ReleaseAccountlock(LockedAccount);
                PatientAccessView_AccountUnLocked(this, new LooseArgs(LockedAccount));
            }
        }

        /// <summary>
        /// Clears the panel.
        /// </summary>
        private void ClearPanel()
        {

            CloseAllOpenDialogs();
            foreach (Control control in Panel.Controls)
            {
                if (control != null)
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch (Exception ex)
                    {
                        c_log.Error("Failed to dispose of a control; " + ex.Message, ex);
                    }
                }
            }
            Panel.Controls.Clear();
        }

        /// <summary>
        /// Closes all open dialogs.
        /// </summary>
        private void CloseAllOpenDialogs()
        {
            foreach (Form form in OwnedForms)
            {
                foreach (Form childForm in form.OwnedForms)
                    childForm.Close();

                if (form.Name != TIMEOUTALERT)
                {
                    form.Close();
                }
            }
        }

        /// <summary>
        /// Displays the announcement view.
        /// </summary>
        private void DisplayAnnouncementView()
        {
            AnnouncementsBaseView announcementBaseView = new AnnouncementsBaseView();
            announcementBaseView.ShowManageAnnouncementView += ShowManagementView;
            announcementBaseView.Dock = DockStyle.Fill;
            Panel.Controls.Add(announcementBaseView);

            DisableIconsAndMenuOptions();
        }

        /// <summary>
        /// Displays the status items.
        /// </summary>
        private void DisplayStatusBarItems()
        {
            sbpUserName.Text = String.Format(" User: {0} ", User.GetCurrent().SecurityUser.UPN);
            sbpFacilityName.Text = String.Format(" Facility: {0} ", User.GetCurrent().Facility.ToCodedString());
            sbpWorkstationId.Text = String.Format(" Workstation: {0} ", User.GetCurrent().WorkstationID);
            sbpVersionNumber.Text = String.Format(" Version: {0} ", Assembly.GetExecutingAssembly().GetName().Version);
        }

        /// <summary>
        /// Does the patient transfer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <param name="currentActivity">The current activity.</param>
        private void DoPatientTransfer(object sender, EventArgs e, Activity currentActivity)
        {
            ClearPanel();
            TransferLocationView transferLocationView = 
                new TransferLocationView(currentActivity) {Dock = DockStyle.Fill};
            Panel.Controls.Add(transferLocationView);
        }
        /// <summary>
        /// Does the patient transfer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <param name="currentActivity">The current activity.</param>
        private void DoPatientTransferERPatToOutpatType(object sender, EventArgs e, Activity currentActivity)
        {
            ClearPanel();
            var emergencyPatientToOutPatientLocationView =
                new EmergencyPatientToOutPatientView(currentActivity) { Dock = DockStyle.Fill };
            Panel.Controls.Add(emergencyPatientToOutPatientLocationView);
        }
        /// <summary>
        /// Does the patient transfer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <param name="currentActivity">The current activity.</param>
        private void DoPatientTransferOutPatientToERPatType(object sender, EventArgs e, Activity currentActivity)
        {
            ClearPanel();
            var outPatientToEmergencyPatientLocationView =
                new OutPatientToEmergencyPatientView(currentActivity) { Dock = DockStyle.Fill };
            Panel.Controls.Add(outPatientToEmergencyPatientLocationView);
        }
       
        private void AssignSelectedCensus(int selectedCensus)
        {
            CensusView censusView = new CensusView();
            try
            {
                censusView.SelectedMenu = selectedCensus;
                censusView.ShowDialog(this);
            }
            finally
            {
                censusView.Dispose();
            }
        }

        private void AssignInquiryString(string inquiryString)
        {
            FindaPhysicianView findaPhysicianView = new FindaPhysicianView();
            try
            {
                findaPhysicianView.SelectedMenu = inquiryString;
                findaPhysicianView.ShowDialog(this);
            }
            finally
            {
                findaPhysicianView.Dispose();
            }
        }

        private void ShowManagementView(object sender, EventArgs e)
        {
            ClearPanel();
            ManageAnnouncementsView manageAnnouncementsView = new ManageAnnouncementsView {Dock = DockStyle.Fill};
            Panel.Controls.Add(manageAnnouncementsView);
        }

        private void AccountLockTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (LockedAccount.Activity.Timeout.SecondAlertTime == 0)
            {
                IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
                ActivityTimeout actTimeout = broker.AccountLockTimeoutFor(
                    new RegistrationActivity());
                LockedAccount.Activity.Timeout = actTimeout;
                LockedAccount.Activity.Timeout.SecondAlertTime =
                    actTimeout.SecondAlertTime;
            }
            StopAccountLockTimer();
            if (timeoutAlertView == null || timeoutAlertView.IsDisposed || timeoutAlertView.Disposing)
            {
                timeoutAlertView = new TimeoutAlertView();
            }
            timeoutAlertView.Model = LockedAccount;
            timeoutAlertView.UpdateModel();
            timeoutAlertView.UpdateView();
            timeoutAlertView.Show();
        }
        private void StartAccountLockTimer(int timeout)
        {
            if (timeout == 0)
            {
                IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

                // replace this call while implementing the activity specific 
                //timeouts. Added a dummy activity
                ActivityTimeout actTimeout = broker.AccountLockTimeoutFor(
                    new RegistrationActivity());

                // work around for setting activity after the broker call
                LockedAccount.Activity.Timeout = actTimeout;
                timeout = actTimeout.FirstAlertTime;
            }

            if (timeout > 0)
            {
                if (i_AccountLockTimer == null)
                {
                    i_AccountLockTimer = new Timer {SynchronizingObject = this};
                    i_AccountLockTimer.Elapsed += AccountLockTimer_Tick;
                }

                i_AccountLockTimer.Interval = timeout;

                i_AccountLockTimer.Start();
            }
        }

        private void StopAccountLockTimer()
        {
            if (i_AccountLockTimer != null)
            {
                i_AccountLockTimer.Stop();
                i_AccountLockTimer = null;
            }
        }

        private bool ConfirmedMenuClick()
        {
            return ConfirmedMenuClick(false);
        }

        private bool ConfirmedMenuClick(bool isClosing)
        {
            bool returnResult = true;
            Domain.Facility facility = User.GetCurrent().Facility;

            if (IsActivityStarted)
            {
                DialogResult result = DialogResult.None;
                if (ShowWarning)
                {
                    if (AccountView.GetInstance().SavingAccountBackgroundWorker != null
                        && AccountView.GetInstance().SavingAccountBackgroundWorker.IsBusy)
                    {
                        result = MessageBox.Show(
                            UIErrorMessages.INCOMPLETE_SAVE_ACTIVITY_MSG,
                            UIErrorMessages.ACTIVITY_DIALOG_TITLE,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        result = MessageBox.Show(
                            UIErrorMessages.INCOMPLETE_ACTIVITY_MSG,
                            UIErrorMessages.ACTIVITY_DIALOG_TITLE,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    // If we got here, we are terminating because of a timeout
                    result = DialogResult.Yes;
                }
                if (result == DialogResult.Yes)
                {
                    ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent(
                        this, new LooseArgs(LockedAccount));

                    //Disable all icons/menu options for FUS notes and Previously scanned documents
                    DisableIconsAndMenuOptions();
                    AccountView.CloseVIweb();
                }
                else
                {
                    returnResult = false;
                }
            }

            else
            {
                AccountView.CloseVIweb();
            }
            if (isClosing)
            {
                ClearVIwebSessionID();
            }

            return returnResult;
        }
        private static void ClearVIwebSessionID()
        {
            var pBarEmployeeID = User.GetCurrent().PBAREmployeeID;
            IVIWebServiceBroker viwebbroker = BrokerFactory.BrokerOfType<IVIWebServiceBroker>();
            viwebbroker.ClearViwebSessionID(pBarEmployeeID);
        }
        private void SelectControlWithFocus(Control control, bool isSelectAll)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl.HasChildren && controlWithFocus == null)
                {
                    SelectControlWithFocus(childControl, isSelectAll);
                }
                else if (childControl.GetType().Name.Equals(MASKEDITTEXTBOX))
                {
                    bool isCurrentControl = false;
                    if (isSelectAll)
                    {
                        isCurrentControl = ((TextBox)(childControl)).TextLength > 0
                            && ((childControl)).ContainsFocus;
                    }
                    else
                    {
                        isCurrentControl = ((childControl)).ContainsFocus;
                    }
                    if (isCurrentControl)
                    {
                        controlWithFocus = childControl;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
      
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Initialize PatientAccessView, call PatientAccessView_Load in Event Handlers.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( PatientAccessView ) );
            this.mainMenu = new System.Windows.Forms.MainMenu( this.components );
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileSeparator0 = new System.Windows.Forms.MenuItem();
            this.mnuHome = new System.Windows.Forms.MenuItem();
            this.mnuFileLogOff = new System.Windows.Forms.MenuItem();
            this.mnuFileLogOffExit = new System.Windows.Forms.MenuItem();
            this.mnuEdit = new System.Windows.Forms.MenuItem();
            this.mnuUndo = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuCut = new System.Windows.Forms.MenuItem();
            this.mnuCopy = new System.Windows.Forms.MenuItem();
            this.mnuPaste = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.mnuSelectAll = new System.Windows.Forms.MenuItem();
            this.mnuView = new System.Windows.Forms.MenuItem();
            this.mnuViewINSCARD = new System.Windows.Forms.MenuItem();
            this.mnuViewDL = new System.Windows.Forms.MenuItem();
            this.mnuViewNPP = new System.Windows.Forms.MenuItem();
            this.mnuViewADV = new System.Windows.Forms.MenuItem();
            this.mnuViewSeparator = new System.Windows.Forms.MenuItem();
            this.mnuViewFUSNotes = new System.Windows.Forms.MenuItem();
            this.mnuRegister = new System.Windows.Forms.MenuItem();
            this.mnuRegisterOnlinePreRegistration = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mnuQuickAccountCreation = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mnuPAIWalkInOutpatientAccountCreation = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator0 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterPreRegisterPatient = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterRegisterPatient = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator2 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterShortPreRegisterPatient = new System.Windows.Forms.MenuItem();
            this.mnuRegisterShortRegisterPatient = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator3 = new System.Windows.Forms.MenuItem();
            this.menuRegisterPatientPreMse = new System.Windows.Forms.MenuItem();
            this.menuRegisterPatientPostMse = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator4 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterPreRegisterNewborn = new System.Windows.Forms.MenuItem();
            this.mnuRegisterRegisterNewborn = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator5 = new System.Windows.Forms.MenuItem();
            this.menuRegisterPatientUCCPreMse = new System.Windows.Forms.MenuItem();
            this.menuRegisterPatientUCCPostMse = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator6 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterEditAccount = new System.Windows.Forms.MenuItem();
            this.mnuPrintFaceSheet = new System.Windows.Forms.MenuItem();
            this.mnuRegisterViewAccount = new System.Windows.Forms.MenuItem();
            this.mnuRegisterSeparator7 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterCancelPreregistration = new System.Windows.Forms.MenuItem();
            this.mnuRegisterCancelInpatientStatus = new System.Windows.Forms.MenuItem();
            this.menuRegisterSeparator8 = new System.Windows.Forms.MenuItem();
            this.mnuRegisterPreRegistrationOffline = new System.Windows.Forms.MenuItem();
            this.mnuRegisterRegistrationOffline = new System.Windows.Forms.MenuItem();
            this.mnuRegisterRegisterPreMSEOffline = new System.Windows.Forms.MenuItem();
            this.mnuRegisterRegisterNewbornOffline = new System.Windows.Forms.MenuItem();
            this.mnuRegisterPreRegisterNewbornOffline = new System.Windows.Forms.MenuItem();
            this.mnuRegisterShortPreRegistrationOffline = new System.Windows.Forms.MenuItem();
            this.mnuRegisterShortRegistrationOffline = new System.Windows.Forms.MenuItem();
            this.mnuDischarge = new System.Windows.Forms.MenuItem();
            this.mnuDischargePreDischarge = new System.Windows.Forms.MenuItem();
            this.mnuDischargeDischargePatient = new System.Windows.Forms.MenuItem();
            this.mnuDischargeEditDischarge = new System.Windows.Forms.MenuItem();
            this.mnuEditRecurringOutpatient = new System.Windows.Forms.MenuItem();
            this.mnuDischargeCancelDischarge = new System.Windows.Forms.MenuItem();
            this.mnuDischargeCancelOutpatientDischarge = new System.Windows.Forms.MenuItem();
            this.mnuTransfer = new System.Windows.Forms.MenuItem();
            this.mnuTransferPatientLocation = new System.Windows.Forms.MenuItem();
            this.mnuTransferOutPatToInPat = new System.Windows.Forms.MenuItem();
            this.mnuTransferInPatToOutPat = new System.Windows.Forms.MenuItem();
            this.mnuTransferERPatientToOutpatient = new System.Windows.Forms.MenuItem();
            this.mnuTransferOutpatientToERPatient = new System.Windows.Forms.MenuItem();
            this.mnuTransferSwapPatientLocations = new System.Windows.Forms.MenuItem();
            this.mnuWorklists = new System.Windows.Forms.MenuItem();
            this.mnuWorklistsPreRegWorklist = new System.Windows.Forms.MenuItem();
            this.mnuWorklistsPostRegWorklist = new System.Windows.Forms.MenuItem();
            this.mnuWorklistsInsuranceWorklist = new System.Windows.Forms.MenuItem();
            this.mnuWorklistsLiabilityWorklist = new System.Windows.Forms.MenuItem();
            this.mnuWorklistsPreMseWorklist = new System.Windows.Forms.MenuItem();
            this.mnuWorklistsNoShowWorklist = new System.Windows.Forms.MenuItem();
            this.mnuCensus = new System.Windows.Forms.MenuItem();
            this.mnuCensusByPatient = new System.Windows.Forms.MenuItem();
            this.mnuCensusbyNursingStation = new System.Windows.Forms.MenuItem();
            this.mnuCensusbyADT = new System.Windows.Forms.MenuItem();
            this.mnuCensusbyPhysician = new System.Windows.Forms.MenuItem();
            this.mnuCensusbyBloodless = new System.Windows.Forms.MenuItem();
            this.mnuCensusbyReligion = new System.Windows.Forms.MenuItem();
            this.mnuCensusbyPayorBroker = new System.Windows.Forms.MenuItem();
            this.mnuReports = new System.Windows.Forms.MenuItem();
            this.mnuReportsPhysicians = new System.Windows.Forms.MenuItem();
            this.mnuAdmin = new System.Windows.Forms.MenuItem();
            this.mnuManageNewEmployers = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuPatientAccessHelp = new System.Windows.Forms.MenuItem();
            this.mnuAboutPatientAccess = new System.Windows.Forms.MenuItem();
            this.stbMain = new System.Windows.Forms.StatusBar();
            this.sbpUserName = new System.Windows.Forms.StatusBarPanel();
            this.sbpFacilityName = new System.Windows.Forms.StatusBarPanel();
            this.sbpWorkstationId = new System.Windows.Forms.StatusBarPanel();
            this.sbpSpacer = new System.Windows.Forms.StatusBarPanel();
            this.sbpVersionNumber = new System.Windows.Forms.StatusBarPanel();
            this.sbpAnotherSpacer = new System.Windows.Forms.StatusBarPanel();
            this.mainToolBar = new System.Windows.Forms.ToolBar();
            this.btnCut = new System.Windows.Forms.ToolBarButton();
            this.btnCopy = new System.Windows.Forms.ToolBarButton();
            this.btnPaste = new System.Windows.Forms.ToolBarButton();
            this.toolBarSpacer = new System.Windows.Forms.ToolBarButton();
            this.btnINSCARD = new System.Windows.Forms.ToolBarButton();
            this.btnDL = new System.Windows.Forms.ToolBarButton();
            this.btnNPP = new System.Windows.Forms.ToolBarButton();
            this.btnADV = new System.Windows.Forms.ToolBarButton();
            this.toolBarSpacer2 = new System.Windows.Forms.ToolBarButton();
            this.btnFUSNotes = new System.Windows.Forms.ToolBarButton();
            this.btnViewOnlinePreRegSupplementalInformation = new System.Windows.Forms.ToolBarButton();
            this.imageList = new System.Windows.Forms.ImageList( this.components );
            this.mainPanel = new System.Windows.Forms.Panel();
            this.textbox = new Extensions.UI.Winforms.MaskedEditTextBox();
         
            ( (System.ComponentModel.ISupportInitialize)( this.sbpUserName ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpFacilityName ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpWorkstationId ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpSpacer ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpVersionNumber ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpAnotherSpacer ) ).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuRegister,
            this.mnuDischarge,
            this.mnuTransfer,
            this.mnuWorklists,
            this.mnuCensus,
            this.mnuReports,
            this.mnuAdmin,
            this.mnuHelp} );
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuFileSeparator0,
            this.mnuHome,
            this.mnuFileLogOff,
            this.mnuFileLogOffExit} );
            this.mnuFile.Text = "&File";
            // 
            // mnuFileSeparator0
            // 
            this.mnuFileSeparator0.Index = 0;
            this.mnuFileSeparator0.Text = "-";
            // 
            // mnuHome
            // 
            this.mnuHome.Index = 1;
            this.mnuHome.Text = "&Home";
            this.mnuHome.Click += new System.EventHandler( this.mnuHome_Click );
            // 
            // mnuFileLogOff
            // 
            this.mnuFileLogOff.Index = 2;
            this.mnuFileLogOff.Text = "&Log Off";
            this.mnuFileLogOff.Click += new System.EventHandler( this.mnuFileLogOff_Click );
            // 
            // mnuFileLogOffExit
            // 
            this.mnuFileLogOffExit.Index = 3;
            this.mnuFileLogOffExit.Text = "E&xit  && Log off";
            this.mnuFileLogOffExit.Click += new System.EventHandler( this.mnuFileLogOffExit_Click );
            // 
            // mnuEdit
            // 
            this.mnuEdit.Index = 1;
            this.mnuEdit.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuUndo,
            this.menuItem1,
            this.mnuCut,
            this.mnuCopy,
            this.mnuPaste,
            this.menuItem2,
            this.mnuSelectAll} );
            this.mnuEdit.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.mnuEdit.Text = "&Edit";
            // 
            // mnuUndo
            // 
            this.mnuUndo.Index = 0;
            this.mnuUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.mnuUndo.Text = "&Undo";
            this.mnuUndo.Click += new System.EventHandler( this.mnuUndo_Click );
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "-";
            // 
            // mnuCut
            // 
            this.mnuCut.Index = 2;
            this.mnuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.mnuCut.Text = "Cu&t";
            this.mnuCut.Click += new System.EventHandler( this.mnuCut_Click );
            // 
            // mnuCopy
            // 
            this.mnuCopy.Index = 3;
            this.mnuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.mnuCopy.Text = "&Copy";
            this.mnuCopy.Click += new System.EventHandler( this.mnuCopy_Click );
            // 
            // mnuPaste
            // 
            this.mnuPaste.Index = 4;
            this.mnuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.mnuPaste.Text = "&Paste";
            this.mnuPaste.Click += new System.EventHandler( this.mnuPaste_Click );
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 5;
            this.menuItem2.Text = "-";
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Index = 6;
            this.mnuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.mnuSelectAll.Text = "Select &All";
            this.mnuSelectAll.Click += new System.EventHandler( this.mnuSelectAll_Click );
            // 
            // mnuView
            // 
            this.mnuView.Index = 2;
            this.mnuView.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuViewINSCARD,
            this.mnuViewDL,
            this.mnuViewNPP,
            this.mnuViewADV,
            this.mnuViewSeparator,
            this.mnuViewFUSNotes} );
            this.mnuView.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.mnuView.Text = "&View";
            // 
            // mnuViewINSCARD
            // 
            this.mnuViewINSCARD.Index = 0;
            this.mnuViewINSCARD.Text = "Insurance Card";
            this.mnuViewINSCARD.Click += new System.EventHandler( this.ViewINSCARD_Docs );
            // 
            // mnuViewDL
            // 
            this.mnuViewDL.Index = 1;
            this.mnuViewDL.Text = "Driver\'s License";
            this.mnuViewDL.Click += new System.EventHandler( this.ViewDL_Docs );
            // 
            // mnuViewNPP
            // 
            this.mnuViewNPP.Index = 2;
            this.mnuViewNPP.Text = "NPP Form";
            this.mnuViewNPP.Click += new System.EventHandler( this.ViewNPP_Docs );
            // 
            // mnuViewADV
            // 
            this.mnuViewADV.Index = 3;
            this.mnuViewADV.Text = "Advanced Directive";
            this.mnuViewADV.Click += new System.EventHandler( this.ViewADV_Docs );
            // 
            // mnuViewSeparator
            // 
            this.mnuViewSeparator.Index = 4;
            this.mnuViewSeparator.Text = "-";
            // 
            // mnuViewFUSNotes
            // 
            this.mnuViewFUSNotes.Index = 5;
            this.mnuViewFUSNotes.Text = "FUS Notes";
            this.mnuViewFUSNotes.Click += new System.EventHandler( this.ViewFUSNotes );
            // 
            // mnuRegister
            // 
            this.mnuRegister.Index = 3;
            this.mnuRegister.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuRegisterOnlinePreRegistration,
            this.menuItem3,
            this.mnuQuickAccountCreation,
            this.menuItem4,
            this.mnuPAIWalkInOutpatientAccountCreation,
            this.mnuRegisterSeparator0,
            this.mnuRegisterPreRegisterPatient,
            this.mnuRegisterSeparator1,
            this.mnuRegisterRegisterPatient,
            this.mnuRegisterSeparator2,
            this.mnuRegisterShortPreRegisterPatient,
            this.mnuRegisterShortRegisterPatient,
            this.mnuRegisterSeparator3,
            this.menuRegisterPatientPreMse,
            this.menuRegisterPatientPostMse,
            this.mnuRegisterSeparator4,
            this.mnuRegisterPreRegisterNewborn,
            this.mnuRegisterRegisterNewborn,
            this.mnuRegisterSeparator5,
            this.menuRegisterPatientUCCPreMse,
            this.menuRegisterPatientUCCPostMse,
            this.mnuRegisterSeparator6,
            this.mnuRegisterEditAccount,
            this.mnuPrintFaceSheet,
            this.mnuRegisterViewAccount,
            this.mnuRegisterSeparator7,
            this.mnuRegisterCancelPreregistration,
            this.mnuRegisterCancelInpatientStatus,
            this.menuRegisterSeparator8,
            this.mnuRegisterPreRegistrationOffline,
            this.mnuRegisterRegistrationOffline,
            this.mnuRegisterRegisterPreMSEOffline,
            this.mnuRegisterRegisterNewbornOffline,
            this.mnuRegisterPreRegisterNewbornOffline,
            this.mnuRegisterShortPreRegistrationOffline,
            this.mnuRegisterShortRegistrationOffline});
            this.mnuRegister.Text = "&Register";
            // 
            // mnuRegisterOnlinePreRegistration
            // 
            this.mnuRegisterOnlinePreRegistration.Index = 0;
            this.mnuRegisterOnlinePreRegistration.Text = "";
            this.mnuRegisterOnlinePreRegistration.Click += new System.EventHandler( this.mnuRegisterOnlinePreRegistration_Click );
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "-";
            // 
            // mnuQuickAccountCreation
            // 
            this.mnuQuickAccountCreation.Index = 2;
            this.mnuQuickAccountCreation.Text = "";
            this.mnuQuickAccountCreation.Click += new System.EventHandler( this.mnuQuickAccountCeation_Click );
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.Text = "-";
            // 
            // mnuPAIWalkInOutpatientAccountCreation
            // 
            this.mnuPAIWalkInOutpatientAccountCreation.Index = 4;
            this.mnuPAIWalkInOutpatientAccountCreation.Text = "";
            this.mnuPAIWalkInOutpatientAccountCreation.Click += new System.EventHandler(this.mnuPAIWalkInOUtpatientAccountCreation_Click);
            // 
            // mnuRegisterSeparator0
            // 
            this.mnuRegisterSeparator0.Index = 5;
            this.mnuRegisterSeparator0.Text = "-";
            // 
            // mnuRegisterPreRegisterPatient
            // 
            this.mnuRegisterPreRegisterPatient.Index = 6;
            this.mnuRegisterPreRegisterPatient.Text = "";
            this.mnuRegisterPreRegisterPatient.Click += new System.EventHandler( this.mnuRegisterPreRegisterPatient_Click );
            // 
            // mnuRegisterSeparator1
            // 
            this.mnuRegisterSeparator1.Index = 7;
            this.mnuRegisterSeparator1.Text = "-";
            // 
            // mnuRegisterRegisterPatient
            // 
            this.mnuRegisterRegisterPatient.Index = 8;
            this.mnuRegisterRegisterPatient.Text = "";
            this.mnuRegisterRegisterPatient.Click += new System.EventHandler( this.mnuRegisterRegisterPatient_Click );
            // 
            // mnuRegisterSeparator2
            // 
            this.mnuRegisterSeparator2.Index = 9;
            this.mnuRegisterSeparator2.Text = "-";
            // 
            // mnuRegisterShortPreRegisterPatient
            // 
            this.mnuRegisterShortPreRegisterPatient.Index = 10;
            this.mnuRegisterShortPreRegisterPatient.Text = "";
            this.mnuRegisterShortPreRegisterPatient.Click += new System.EventHandler( this.mnuRegisterShortPreRegisterPatient_Click );
            // 
            // mnuRegisterShortRegisterPatient
            // 
            this.mnuRegisterShortRegisterPatient.Index = 11;
            this.mnuRegisterShortRegisterPatient.Text = "";
            this.mnuRegisterShortRegisterPatient.Click += new System.EventHandler( this.mnuRegisterShortRegisterPatient_Click );
            // 
            // mnuRegisterSeparator3
            // 
            this.mnuRegisterSeparator3.Index = 12;
            this.mnuRegisterSeparator3.Text = "-";
            // 
            // menuRegisterPatientPreMse
            // 
            this.menuRegisterPatientPreMse.Index = 13;
            this.menuRegisterPatientPreMse.Text = "";
            this.menuRegisterPatientPreMse.Click += new System.EventHandler( this.menuRegisterPatientPreMse_Click );
            // 
            // menuRegisterPatientPostMse
            // 
            this.menuRegisterPatientPostMse.Index = 14;
            this.menuRegisterPatientPostMse.Text = "";
            this.menuRegisterPatientPostMse.Click += new System.EventHandler( this.menuRegisterPatientPostMse_Click );
            // 
            // mnuRegisterSeparator4
            // 
            this.mnuRegisterSeparator4.Index = 15;
            this.mnuRegisterSeparator4.Text = "-";
            //
            // mnuRegisterPreRegisterNewborn
            // 
            this.mnuRegisterPreRegisterNewborn.Index = 16;
            this.mnuRegisterPreRegisterNewborn.Text = "";
            this.mnuRegisterPreRegisterNewborn.Click += new System.EventHandler( this.mnuRegisterPreRegisterNewborn_Click );
            // 
            // mnuRegisterRegisterNewborn
            // 
            this.mnuRegisterRegisterNewborn.Index = 17;
            this.mnuRegisterRegisterNewborn.Text = "";
            this.mnuRegisterRegisterNewborn.Click += new System.EventHandler( this.mnuRegisterRegisterNewborn_Click );
            
            // mnuRegisterSeparator5
            // 
            this.mnuRegisterSeparator5.Index = 18;
            this.mnuRegisterSeparator5.Text = "-";
            // 
            // menuRegisterPatientUCCPreMse
            // 
            this.menuRegisterPatientUCCPreMse.Index = 19;
            this.menuRegisterPatientUCCPreMse.Text = "";
            this.menuRegisterPatientUCCPreMse.Click += new System.EventHandler(this.menuRegisterPatientUCCPreMse_Click);
            // 
            // menuRegisterPatientUCCPostMse
            // 
            this.menuRegisterPatientUCCPostMse.Index = 20;
            this.menuRegisterPatientUCCPostMse.Text = "";
            this.menuRegisterPatientUCCPostMse.Click += new System.EventHandler(this.menuRegisterPatientUCCPostMse_Click);
            // 
            // mnuRegisterSeparator6
            // 
            this.mnuRegisterSeparator6.Index = 21;
            this.mnuRegisterSeparator6.Text = "-";
            // 
            // mnuRegisterEditAccount
            // 
            this.mnuRegisterEditAccount.Index = 22;
            this.mnuRegisterEditAccount.Text = "";
            this.mnuRegisterEditAccount.Click += new System.EventHandler( this.mnuRegisterEditAccount_Click );
            // 
            // mnuPrintFaceSheet
            // 
            this.mnuPrintFaceSheet.Index = 23;
            this.mnuPrintFaceSheet.Text = "";
            this.mnuPrintFaceSheet.Click += new System.EventHandler( this.mnuPrintFaceSheet_Click );
            // 
            // mnuRegisterViewAccount
            // 
            this.mnuRegisterViewAccount.Index = 24;
            this.mnuRegisterViewAccount.Text = "";
            this.mnuRegisterViewAccount.Click += new System.EventHandler( this.mnuRegisterViewAccount_Click );
            // 
            // mnuRegisterSeparator7
            // 
            this.mnuRegisterSeparator7.Index = 25;
            this.mnuRegisterSeparator7.Text = "-";
            // 
            // mnuRegisterCancelPreregistration
            // 
            this.mnuRegisterCancelPreregistration.Index = 26;
            this.mnuRegisterCancelPreregistration.Text = "";
            this.mnuRegisterCancelPreregistration.Click += new System.EventHandler( this.mnuRegisterCancelPreregistration_Click );
            // 
            // mnuRegisterCancelInpatientStatus
            // 
            this.mnuRegisterCancelInpatientStatus.Index = 27;
            this.mnuRegisterCancelInpatientStatus.Text = "";
            this.mnuRegisterCancelInpatientStatus.Click += new System.EventHandler( this.mnuRegisterCancelInpatientStatus_Click );
            // 
            // mnuRegisterSeparator8
            // 
            this.menuRegisterSeparator8.Index = 28;
            this.menuRegisterSeparator8.Text = "-";
            // 
            // mnuRegisterPreRegistrationOffline
            // 
            this.mnuRegisterPreRegistrationOffline.Index = 29;
            this.mnuRegisterPreRegistrationOffline.Text = "";
            this.mnuRegisterPreRegistrationOffline.Click += new System.EventHandler( this.mnuRegisterPreRegistrationOffline_Click );
            // 
            // mnuRegisterRegistrationOffline
            // 
            this.mnuRegisterRegistrationOffline.Index = 30;
            this.mnuRegisterRegistrationOffline.Text = "";
            this.mnuRegisterRegistrationOffline.Click += new System.EventHandler( this.mnuRegisterRegistrationOffline_Click );
            // 
            // mnuRegisterRegisterPreMSEOffline
            // 
            this.mnuRegisterRegisterPreMSEOffline.Index = 31;
            this.mnuRegisterRegisterPreMSEOffline.Text = "";
            this.mnuRegisterRegisterPreMSEOffline.Click += new System.EventHandler( this.mnuRegisterRegisterPreMSEOffline_Click );
            // 
            // mnuRegisterRegisterNewbornOffline
            // 
            this.mnuRegisterRegisterNewbornOffline.Index = 32;
            this.mnuRegisterRegisterNewbornOffline.Text = "";
            this.mnuRegisterRegisterNewbornOffline.Click += new System.EventHandler( this.mnuRegisterRegisterNewbornOffline_Click );
            // 
            // mnuRegisterPreRegisterNewbornOffline
            // 
            this.mnuRegisterPreRegisterNewbornOffline.Index = 33;
            this.mnuRegisterPreRegisterNewbornOffline.Text = "";
            this.mnuRegisterPreRegisterNewbornOffline.Click += new System.EventHandler( this.mnuRegisterPreRegisterNewbornOffline_Click );
            // 
            // mnuRegisterShortPreRegistrationOffline
            // 
            this.mnuRegisterShortPreRegistrationOffline.Index = 34;
            this.mnuRegisterShortPreRegistrationOffline.Text = "";
            this.mnuRegisterShortPreRegistrationOffline.Click += new System.EventHandler(this.mnuRegisterShortPreRegistrationOffline_Click);
            // 
            // mnuRegisterShortRegistrationOffline
            // 
            this.mnuRegisterShortRegistrationOffline.Index = 35;
            this.mnuRegisterShortRegistrationOffline.Text = "";
            this.mnuRegisterShortRegistrationOffline.Click += new System.EventHandler(this.mnuRegisterShortRegistrationOffline_Click);
          
            // 
            // mnuDischarge
            // 
            this.mnuDischarge.Index = 4;
            this.mnuDischarge.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuDischargePreDischarge,
            this.mnuDischargeDischargePatient,
            this.mnuDischargeEditDischarge,
            this.mnuEditRecurringOutpatient,
            this.mnuDischargeCancelDischarge,
            this.mnuDischargeCancelOutpatientDischarge} );
            this.mnuDischarge.Text = "&Discharge";
            // 
            // mnuDischargePreDischarge
            // 
            this.mnuDischargePreDischarge.Index = 0;
            this.mnuDischargePreDischarge.Text = "";
            this.mnuDischargePreDischarge.Click += new System.EventHandler( this.mnuDischargePreDischarge_Click );
            // 
            // mnuDischargeDischargePatient
            // 
            this.mnuDischargeDischargePatient.Index = 1;
            this.mnuDischargeDischargePatient.Text = "";
            this.mnuDischargeDischargePatient.Click += new System.EventHandler( this.mnuDischargePatient_Click );
            // 
            // mnuDischargeEditDischarge
            // 
            this.mnuDischargeEditDischarge.Index = 2;
            this.mnuDischargeEditDischarge.Text = "";
            this.mnuDischargeEditDischarge.Click += new System.EventHandler( this.mnuEditDischarge_Click );
            // 
            // mnuEditRecurringOutpatient
            // 
            this.mnuEditRecurringOutpatient.Index = 3;
            this.mnuEditRecurringOutpatient.Text = "";
            this.mnuEditRecurringOutpatient.Click += new System.EventHandler( this.mnuEditRecurringOutpatient_Click );
            // 
            // mnuDischargeCancelDischarge
            // 
            this.mnuDischargeCancelDischarge.Index = 4;
            this.mnuDischargeCancelDischarge.Text = "";
            this.mnuDischargeCancelDischarge.Click += new System.EventHandler( this.mnuCancelDischarge_Click );
            // 
            // mnuDischargeCancelOutpatientDischarge
            // 
            this.mnuDischargeCancelOutpatientDischarge.Index = 5;
            this.mnuDischargeCancelOutpatientDischarge.Text = "";
            this.mnuDischargeCancelOutpatientDischarge.Click += new System.EventHandler( this.mnuCancelOutpatientDischarge_Click );
            // 
            // mnuTransfer
            // 
            this.mnuTransfer.Index = 5;
            this.mnuTransfer.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuTransferPatientLocation,
            this.mnuTransferOutPatToInPat,
            this.mnuTransferInPatToOutPat,
            this.mnuTransferERPatientToOutpatient,
            this.mnuTransferOutpatientToERPatient,
            this.mnuTransferSwapPatientLocations} );
            this.mnuTransfer.Text = "&Transfer";
            // 
            // mnuTransferPatientLocation
            // 
            this.mnuTransferPatientLocation.Index = 0;
            this.mnuTransferPatientLocation.Text = "";
            this.mnuTransferPatientLocation.Click += new System.EventHandler( this.mnuTransferPatientLocation_Click );
            // 
            // mnuTransferOutPatToInPat
            // 
            this.mnuTransferOutPatToInPat.Index = 1;
            this.mnuTransferOutPatToInPat.Text = "";
            this.mnuTransferOutPatToInPat.Click += new System.EventHandler( this.mnuTransferOutPatToInPat_Click );
            // 
            // mnuTransferInPatToOutPat
            // 
            this.mnuTransferInPatToOutPat.Index = 2;
            this.mnuTransferInPatToOutPat.Text = "";
            this.mnuTransferInPatToOutPat.Click += new System.EventHandler( this.mnuTransferInPatToOutPat_Click );
            // 
            // mnuTransferSwapPatientLocations
            // 
            this.mnuTransferERPatientToOutpatient.Index = 3;
            this.mnuTransferERPatientToOutpatient.Text = "";
            this.mnuTransferERPatientToOutpatient.Click += new System.EventHandler(this.mnuTransferERToOutpatientPatient_Click);
            // 
            // mnuTransferOutpatientToERPatient
            // 
            this.mnuTransferOutpatientToERPatient.Index = 4;
            this.mnuTransferOutpatientToERPatient.Text = "";
            this.mnuTransferOutpatientToERPatient.Click += new System.EventHandler(this.mnuTransferOutpatientToERPatient_Click);
            // 
            // mnuTransferERPatientToOutpatient
            // 
            this.mnuTransferSwapPatientLocations.Index = 5;
            this.mnuTransferSwapPatientLocations.Text = "";
            this.mnuTransferSwapPatientLocations.Click += new System.EventHandler(this.mnuTransferSwapPatientLocations_Click);
            // 
            // mnuWorklists
            // 
            this.mnuWorklists.Index = 6;
            this.mnuWorklists.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuWorklistsPreRegWorklist,
            this.mnuWorklistsPostRegWorklist,
            this.mnuWorklistsInsuranceWorklist,
            this.mnuWorklistsLiabilityWorklist,
            this.mnuWorklistsPreMseWorklist,
            this.mnuWorklistsNoShowWorklist} );
            this.mnuWorklists.Text = "&Worklists";
            // 
            // mnuWorklistsPreRegWorklist
            // 
            this.mnuWorklistsPreRegWorklist.Index = 0;
            this.mnuWorklistsPreRegWorklist.Text = "";
            this.mnuWorklistsPreRegWorklist.Click += new System.EventHandler( this.mnuWorklistsPreRegWorklist_Click );
            // 
            // mnuWorklistsPostRegWorklist
            // 
            this.mnuWorklistsPostRegWorklist.Index = 1;
            this.mnuWorklistsPostRegWorklist.Text = "";
            this.mnuWorklistsPostRegWorklist.Click += new System.EventHandler( this.mnuWorklistsPostRegWorklist_Click );
            // 
            // mnuWorklistsInsuranceWorklist
            // 
            this.mnuWorklistsInsuranceWorklist.Index = 2;
            this.mnuWorklistsInsuranceWorklist.Text = "";
            this.mnuWorklistsInsuranceWorklist.Click += new System.EventHandler( this.mnuWorklistsInsuranceWorklist_Click );
            // 
            // mnuWorklistsLiabilityWorklist
            // 
            this.mnuWorklistsLiabilityWorklist.Index = 3;
            this.mnuWorklistsLiabilityWorklist.Text = "";
            this.mnuWorklistsLiabilityWorklist.Click += new System.EventHandler( this.mnuWorklistsLiabilityWorklist_Click );
            // 
            // mnuWorklistsPreMseWorklist
            // 
            this.mnuWorklistsPreMseWorklist.Index = 4;
            this.mnuWorklistsPreMseWorklist.Text = "";
            this.mnuWorklistsPreMseWorklist.Click += new System.EventHandler( this.mnuWorklistsPreMseWorklist_Click );
            // 
            // mnuWorklistsNoShowWorklist
            // 
            this.mnuWorklistsNoShowWorklist.Index = 5;
            this.mnuWorklistsNoShowWorklist.Text = "";
            this.mnuWorklistsNoShowWorklist.Click += new System.EventHandler( this.mnuWorklistsNoShowWorklist_Click );
            // 
            // mnuCensus
            // 
            this.mnuCensus.Index = 7;
            this.mnuCensus.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuCensusByPatient,
            this.mnuCensusbyNursingStation,
            this.mnuCensusbyADT,
            this.mnuCensusbyPhysician,
            this.mnuCensusbyBloodless,
            this.mnuCensusbyReligion,
            this.mnuCensusbyPayorBroker} );
            this.mnuCensus.Text = "&Census";
            // 
            // mnuCensusByPatient
            // 
            this.mnuCensusByPatient.Index = 0;
            this.mnuCensusByPatient.Text = "";
            this.mnuCensusByPatient.Click += new System.EventHandler( this.mnuCensusByPatient_Click );
            // 
            // mnuCensusbyNursingStation
            // 
            this.mnuCensusbyNursingStation.Index = 1;
            this.mnuCensusbyNursingStation.Text = "";
            this.mnuCensusbyNursingStation.Click += new System.EventHandler( this.mnuCensusbyNursingStation_Click );
            // 
            // mnuCensusbyADT
            // 
            this.mnuCensusbyADT.Index = 2;
            this.mnuCensusbyADT.Text = "";
            this.mnuCensusbyADT.Click += new System.EventHandler( this.mnuCensusbyADT_Click );
            // 
            // mnuCensusbyPhysician
            // 
            this.mnuCensusbyPhysician.Index = 3;
            this.mnuCensusbyPhysician.Text = "";
            this.mnuCensusbyPhysician.Click += new System.EventHandler( this.mnuCensusbyPhysician_Click );
            // 
            // mnuCensusbyBloodless
            // 
            this.mnuCensusbyBloodless.Index = 4;
            this.mnuCensusbyBloodless.Text = "";
            this.mnuCensusbyBloodless.Click += new System.EventHandler( this.mnuCensusbyBloodless_Click );
            // 
            // mnuCensusbyReligion
            // 
            this.mnuCensusbyReligion.Index = 5;
            this.mnuCensusbyReligion.Text = "";
            this.mnuCensusbyReligion.Click += new System.EventHandler( this.mnuCensusbyReligion_Click );
            // 
            // mnuCensusbyPayorBroker
            // 
            this.mnuCensusbyPayorBroker.Index = 6;
            this.mnuCensusbyPayorBroker.Text = "";
            this.mnuCensusbyPayorBroker.Click += new System.EventHandler( this.mnuCensusbyPayorBroker_Click );
            // 
            // mnuReports
            // 
            this.mnuReports.Index = 8;
            this.mnuReports.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuReportsPhysicians} );
            this.mnuReports.Text = "Rep&orts";
            // 
            // mnuReportsPhysicians
            // 
            this.mnuReportsPhysicians.Index = 0;
            this.mnuReportsPhysicians.Text = "";
            this.mnuReportsPhysicians.Click += new System.EventHandler( this.mnuReportsPhysicians_Click );
            // 
            // mnuAdmin
            // 
            this.mnuAdmin.Index = 9;
            this.mnuAdmin.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuManageNewEmployers} );
            this.mnuAdmin.Text = "Ad&min";
            // 
            // mnuManageNewEmployers
            // 
            this.mnuManageNewEmployers.Index = 0;
            this.mnuManageNewEmployers.Text = "Manage &New Employers";
            this.mnuManageNewEmployers.Click += new System.EventHandler( this.mnuNewEmployerManagement_Click );
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 10;
            this.mnuHelp.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.mnuPatientAccessHelp,
            this.mnuAboutPatientAccess} );
            this.mnuHelp.Text = "&Help";
            // 
            // mnuPatientAccessHelp
            // 
            this.mnuPatientAccessHelp.Index = 0;
            this.mnuPatientAccessHelp.Text = "Patient Access &Help";
            this.mnuPatientAccessHelp.Click += new System.EventHandler( this.mnuPatientAccessHelp_Click );
            // 
            // mnuAboutPatientAccess
            // 
            this.mnuAboutPatientAccess.Index = 1;
            this.mnuAboutPatientAccess.Text = "&About Patient Access";
            this.mnuAboutPatientAccess.Click += new System.EventHandler( this.mnuAboutPatientAccess_Click );
            // 
            // stbMain
            // 
            this.stbMain.Location = new System.Drawing.Point( 0, 647 );
            this.stbMain.Name = "stbMain";
            this.stbMain.Panels.AddRange( new System.Windows.Forms.StatusBarPanel[] {
            this.sbpUserName,
            this.sbpFacilityName,
            this.sbpWorkstationId,
            this.sbpSpacer,
            this.sbpVersionNumber,
            this.sbpAnotherSpacer} );
            this.stbMain.ShowPanels = true;
            this.stbMain.Size = new System.Drawing.Size( 1025, 20 );
            this.stbMain.TabIndex = 0;
            // 
            // sbpUserName
            // 
            this.sbpUserName.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpUserName.MinWidth = 60;
            this.sbpUserName.Name = "sbpUserName";
            this.sbpUserName.Width = 60;
            // 
            // sbpFacilityName
            // 
            this.sbpFacilityName.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpFacilityName.MinWidth = 100;
            this.sbpFacilityName.Name = "sbpFacilityName";
            // 
            // sbpWorkstationId
            // 
            this.sbpWorkstationId.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpWorkstationId.MinWidth = 60;
            this.sbpWorkstationId.Name = "sbpWorkstationId";
            this.sbpWorkstationId.Width = 60;
            // 
            // sbpSpacer
            // 
            this.sbpSpacer.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.sbpSpacer.MinWidth = 100;
            this.sbpSpacer.Name = "sbpSpacer";
            this.sbpSpacer.Width = 718;
            // 
            // sbpVersionNumber
            // 
            this.sbpVersionNumber.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
            this.sbpVersionNumber.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpVersionNumber.MinWidth = 60;
            this.sbpVersionNumber.Name = "sbpVersionNumber";
            this.sbpVersionNumber.Width = 60;
            // 
            // sbpAnotherSpacer
            // 
            this.sbpAnotherSpacer.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
            this.sbpAnotherSpacer.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.sbpAnotherSpacer.Name = "sbpAnotherSpacer";
            this.sbpAnotherSpacer.Width = 10;
            // 
            // mainToolBar
            // 
            this.mainToolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.mainToolBar.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 94 ) ) ) ), ( (int)( ( (byte)( 137 ) ) ) ), ( (int)( ( (byte)( 182 ) ) ) ) );
            this.mainToolBar.Buttons.AddRange( new System.Windows.Forms.ToolBarButton[] {
            this.btnCut,
            this.btnCopy,
            this.btnPaste,
            this.toolBarSpacer,
            this.btnINSCARD,
            this.btnDL,
            this.btnNPP,
            this.btnADV,
            this.toolBarSpacer2,
            this.btnFUSNotes,
            this.btnViewOnlinePreRegSupplementalInformation} );
            this.mainToolBar.ButtonSize = new System.Drawing.Size( 28, 26 );
            this.mainToolBar.Divider = false;
            this.mainToolBar.DropDownArrows = true;
            this.mainToolBar.ImageList = this.imageList;
            this.mainToolBar.Location = new System.Drawing.Point( 0, 0 );
            this.mainToolBar.Margin = new System.Windows.Forms.Padding( 3, 10, 3, 5 );
            this.mainToolBar.Name = "mainToolBar";
            this.mainToolBar.ShowToolTips = true;
            this.mainToolBar.Size = new System.Drawing.Size( 1025, 30 );
            this.mainToolBar.TabIndex = 1;
            this.mainToolBar.Wrappable = false;
            this.mainToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler( this.mainToolBar_ButtonClick );
            this.mainToolBar.MouseEnter += new System.EventHandler( this.mainToolBar_MouseEnter );
            this.mainToolBar.MouseLeave += new System.EventHandler( this.mainToolBar_MouseLeave );
            // 
            // btnCut
            // 
            this.btnCut.ImageIndex = 0;
            this.btnCut.Name = "btnCut";
            this.btnCut.ToolTipText = "Cut";
            // 
            // btnCopy
            // 
            this.btnCopy.ImageIndex = 1;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.ToolTipText = "Copy";
            // 
            // btnPaste
            // 
            this.btnPaste.ImageIndex = 2;
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.ToolTipText = "Paste";
            // 
            // toolBarSpacer
            // 
            this.toolBarSpacer.Name = "toolBarSpacer";
            this.toolBarSpacer.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnINSCARD
            // 
            this.btnINSCARD.ImageIndex = 3;
            this.btnINSCARD.Name = "btnINSCARD";
            this.btnINSCARD.ToolTipText = "Insurance Card";
            // 
            // btnDL
            // 
            this.btnDL.ImageIndex = 4;
            this.btnDL.Name = "btnDL";
            this.btnDL.ToolTipText = "Driver\'s License";
            // 
            // btnNPP
            // 
            this.btnNPP.ImageIndex = 5;
            this.btnNPP.Name = "btnNPP";
            this.btnNPP.ToolTipText = "NPP Form";
            // 
            // btnADV
            // 
            this.btnADV.ImageIndex = 6;
            this.btnADV.Name = "btnADV";
            this.btnADV.ToolTipText = "Advance Directive";
            // 
            // toolBarSpacer2
            // 
            this.toolBarSpacer2.Name = "toolBarSpacer2";
            this.toolBarSpacer2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnFUSNotes
            // 
            this.btnFUSNotes.ImageIndex = 7;
            this.btnFUSNotes.Name = "btnFUSNotes";
            this.btnFUSNotes.ToolTipText = "FUS Notes";
            // 
            // btnViewOnlinePreRegSupplementalInformation
            // 
            this.btnViewOnlinePreRegSupplementalInformation.Enabled = false;
            this.btnViewOnlinePreRegSupplementalInformation.ImageIndex = 16;
            this.btnViewOnlinePreRegSupplementalInformation.Name = "btnViewOnlinePreRegSupplementalInformation";
            this.btnViewOnlinePreRegSupplementalInformation.ToolTipText = "View Online PreReg Supplemental Information";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ( (System.Windows.Forms.ImageListStreamer)( resources.GetObject( "imageList.ImageStream" ) ) );
            this.imageList.TransparentColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 94 ) ) ) ), ( (int)( ( (byte)( 137 ) ) ) ), ( (int)( ( (byte)( 185 ) ) ) ) );
            this.imageList.Images.SetKeyName( 0, "Cut_Enabled.ico" );
            this.imageList.Images.SetKeyName( 1, "Copy_Enabled.ico" );
            this.imageList.Images.SetKeyName( 2, "Paste_Enabled.ico" );
            this.imageList.Images.SetKeyName( 3, "InsuranceCard_Enabled.ico" );
            this.imageList.Images.SetKeyName( 4, "DriversLicense_Enabled.ico" );
            this.imageList.Images.SetKeyName( 5, "NPP_Enabled.ico" );
            this.imageList.Images.SetKeyName( 6, "AdvancedDirective_Enabled.ico" );
            this.imageList.Images.SetKeyName( 7, "FUSNotes_Enabled.ico" );
            this.imageList.Images.SetKeyName( 8, "Cut_Disabled.ico" );
            this.imageList.Images.SetKeyName( 9, "Copy_Disabled.ico" );
            this.imageList.Images.SetKeyName( 10, "Paste_Disabled.ico" );
            this.imageList.Images.SetKeyName( 11, "InsuranceCard_Disabled.ico" );
            this.imageList.Images.SetKeyName( 12, "DriversLicense_Disabled.ico" );
            this.imageList.Images.SetKeyName( 13, "NPP_Disabled.ico" );
            this.imageList.Images.SetKeyName( 14, "AdvancedDirective_Disabled.ico" );
            this.imageList.Images.SetKeyName( 15, "FUSNotes_Disabled.ico" );
            this.imageList.Images.SetKeyName( 16, "OnlinePreregistrationSupplementalInformationView.ico" );
            // 
            // mainPanel
            // 
            this.mainPanel.AutoScroll = true;
            this.mainPanel.AutoScrollMinSize = new System.Drawing.Size( 800, 600 );
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.Location = new System.Drawing.Point( 0, 28 );
            this.mainPanel.Margin = new System.Windows.Forms.Padding( 2, 5, 2, 5 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 1024, 619 );
            this.mainPanel.TabIndex = 0;
            // 
            // textbox
            // 
            this.textbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.textbox.Location = new System.Drawing.Point( 0, 0 );
            this.textbox.Mask = "";
            this.textbox.Name = "textbox";
            this.textbox.Size = new System.Drawing.Size( 100, 20 );
            this.textbox.TabIndex = 0;
          
            // 
            // PatientAccessView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 94 ) ) ) ), ( (int)( ( (byte)( 137 ) ) ) ), ( (int)( ( (byte)( 185 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 1025, 667 );
            this.Controls.Add( this.mainPanel );
            this.Controls.Add( this.stbMain );
            this.Controls.Add( this.mainToolBar );
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.Menu = this.mainMenu;
            this.Name = "PatientAccessView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patient Access";
            this.Closing += new System.ComponentModel.CancelEventHandler( this.PatientAccessView_Closing );
            this.Load += new System.EventHandler( this.PatientAccessView_Load );
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.On_PatientAccessViewPaint );
            this.Resize += new System.EventHandler( this.PatientAccess_Resize );
            ( (System.ComponentModel.ISupportInitialize)( this.sbpUserName ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpFacilityName ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpWorkstationId ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpSpacer ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpVersionNumber ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.sbpAnotherSpacer ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        #region Private Properties
        protected internal Panel Panel
        {
            get
            {
                return mainPanel;
            }
            set
            {
                mainPanel = value;
            }
        }
        protected MaskedEditTextBox TextBox
        {
            get
            {
                return textbox;
            }
            set
            {
                textbox = value;
            }
        }

        private bool IsActivityStarted
        {
            get
            {
                return i_ActivityStarted;
            }
            set
            {
                i_ActivityStarted = value;
            }
        }

        private Location LockedBedLocation
        {
            get
            {
                return i_LockedBedLocation;
            }
            set
            {
                i_LockedBedLocation = value;
            }
        }

        private IAccount LockedAccount
        {
            get
            {
                return i_LockedAccount;
            }
            set
            {
                i_LockedAccount = value;
            }
        }

        private IAccount LockedAccount2
        {
            get
            {
                return i_LockedAccount2;
            }
            set
            {
                i_LockedAccount2 = value;
            }
        }

        private VIWEBFeatureManager VIWEBFeatureManager { get; set; }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construction for PatientAccessView, 
        /// calls InitializeComponent in Private Methods region.
        /// </summary>
        public PatientAccessView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().PreMseRegistrationEvent +=
                menuRegisterPatientPreMse_Click;

            SearchEventAggregator.GetInstance().UCPreMseRegistrationEvent +=
                menuRegisterPatientUCCPreMse_Click;

            SearchEventAggregator.GetInstance().CancelPreRegistrationEvent +=
                mnuRegisterCancelPreregistration_Click;

            PatientAccessViewPopulationAggregator.GetInstance().ActionSelected +=
                ActionSelectedEventHandler;

            ActivityEventAggregator.GetInstance().ActivityStarted +=
                PatientAccessView_ActivityStarted;

            ActivityEventAggregator.GetInstance().ActivityCompleted +=
                PatientAccessView_ActivityCompleted;

            ActivityEventAggregator.GetInstance().AccountLocked +=
                PatientAccessView_AccountLocked;

            ActivityEventAggregator.GetInstance().AccountUnLocked +=
                PatientAccessView_AccountUnLocked;

            ActivityEventAggregator.GetInstance().BedLocked +=
                PatientAccessView_BedLocked;

            ActivityEventAggregator.GetInstance().ActivityCancelled +=
                OnActivityCancelled;

            ActivityEventAggregator.GetInstance().ReturnToMainScreen +=
                PatientAccessView_ReturnToMainScreen;
            SearchEventAggregator.GetInstance().EditUCPreMSEAccount +=
                mnuRegisterEditAccount_Click;
            SearchEventAggregator.GetInstance().EditEDPreMSEAccount +=
                mnuRegisterEditAccount_Click;
            InitializeTimeOutActivity();
  
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            SearchEventAggregator.GetInstance().RemoveAllListeners();
            ActivityEventAggregator.GetInstance().RemoveAllListeners();
            PatientAccessViewPopulationAggregator.GetInstance().RemoveAllListeners();

            ActivityEventAggregator.GetInstance().ReturnToMainScreen -=
                PatientAccessView_ReturnToMainScreen;
            
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            CancelBackgroundWorker();
            base.Dispose(disposing);
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger(typeof(PatientAccessView));

        private IContainer components;

        private StatusBar stbMain;

        private ToolBar mainToolBar;

        private ToolBarButton btnCopy;
        private ToolBarButton btnCut;
        private ToolBarButton btnPaste;
        private ToolBarButton toolBarSpacer;
        private ToolBarButton btnINSCARD;
        private ToolBarButton btnDL;
        private ToolBarButton btnNPP;
        private ToolBarButton btnADV;

        private ImageList imageList;

        private Panel mainPanel;
        private MaskedEditTextBox textbox;
        private Size minClientSize;

        private WorklistsView worklistsView;
        private MaintenanceCmdView maintenanceCmdView;
        private TimeoutAlertView timeoutAlertView;
        private DischargeView dischargeView;
        // ADD MENU FIELDS WITH FORMAT: mnuTop-LevelName followed by drop-down command name

        // Top-level menu
        private MainMenu mainMenu;
        private MenuItem mnuFileSeparator0;
        // File menu
        private MenuItem mnuFile;
        // Edit menu
        private MenuItem mnuEdit;
        // View menu
        private MenuItem mnuView;
        private MenuItem mnuViewADV;
        private MenuItem mnuViewDL;
        private MenuItem mnuViewINSCARD;
        private MenuItem mnuViewNPP;
        private MenuItem mnuViewFUSNotes;
        private MenuItem mnuViewSeparator;
        // Register menu
        private MenuItem mnuRegisterOnlinePreRegistration;
        private MenuItem mnuRegisterSeparator6;
        private MenuItem mnuRegister;
        private MenuItem mnuRegisterPreRegisterPatient;
        private MenuItem mnuRegisterSeparator0;
        private MenuItem mnuRegisterRegisterPatient;
        private MenuItem mnuRegisterSeparator1;
        private MenuItem mnuRegisterShortPreRegisterPatient;
        private MenuItem mnuRegisterShortRegisterPatient;
        private MenuItem mnuRegisterSeparator7;
        private MenuItem menuRegisterPatientPreMse;
        private MenuItem menuRegisterPatientPostMse;
        private MenuItem mnuRegisterSeparator2;
        private MenuItem mnuRegisterRegisterNewborn;
        private MenuItem mnuRegisterPreRegisterNewborn;
        private MenuItem menuRegisterSeparator8;
        private MenuItem menuRegisterPatientUCCPreMse;
        private MenuItem menuRegisterPatientUCCPostMse;
        private MenuItem mnuRegisterSeparator3;
        private MenuItem mnuRegisterEditAccount;
        private MenuItem mnuPrintFaceSheet;
        private MenuItem mnuRegisterViewAccount;
        private MenuItem mnuRegisterSeparator4;
        private MenuItem mnuRegisterCancelPreregistration;
        private MenuItem mnuRegisterCancelInpatientStatus;
        private MenuItem mnuRegisterSeparator5;
        private MenuItem mnuRegisterPreRegistrationOffline;
        private MenuItem mnuRegisterRegistrationOffline;
        private MenuItem mnuRegisterShortPreRegistrationOffline;
        private MenuItem mnuRegisterShortRegistrationOffline;
        private MenuItem mnuRegisterRegisterPreMSEOffline;
        private MenuItem mnuRegisterRegisterNewbornOffline;
        private MenuItem mnuRegisterPreRegisterNewbornOffline;
        // Discharge menu
        private MenuItem mnuDischarge;
        private MenuItem mnuDischargePreDischarge;
        private MenuItem mnuDischargeDischargePatient;
        private MenuItem mnuDischargeEditDischarge;
        private MenuItem mnuDischargeCancelDischarge;
        private MenuItem mnuDischargeCancelOutpatientDischarge;
        // Transfer menu
        private MenuItem mnuTransfer;
        private MenuItem mnuTransferPatientLocation;
        private MenuItem mnuTransferInPatToOutPat;
        private MenuItem mnuTransferOutPatToInPat;
        private MenuItem mnuTransferERPatientToOutpatient;
        private MenuItem mnuTransferOutpatientToERPatient;
        private MenuItem mnuTransferSwapPatientLocations;
        // Worklists menu
        private MenuItem mnuWorklists;
        private MenuItem mnuWorklistsPreRegWorklist;
        private MenuItem mnuWorklistsPostRegWorklist;
        private MenuItem mnuWorklistsInsuranceWorklist;
        private MenuItem mnuWorklistsLiabilityWorklist;
        private MenuItem mnuWorklistsPreMseWorklist;
        private MenuItem mnuWorklistsNoShowWorklist;
        // Census
        private MenuItem mnuCensus;
        private MenuItem mnuCensusByPatient;
        private MenuItem mnuCensusbyNursingStation;
        private MenuItem mnuCensusbyADT;
        private MenuItem mnuCensusbyPhysician;
        private MenuItem mnuCensusbyBloodless;
        private MenuItem mnuCensusbyReligion;
        private MenuItem mnuCensusbyPayorBroker;
        // Reports menu
        private MenuItem mnuReports;
        private MenuItem mnuReportsPhysicians;
        // Help menu
        private MenuItem mnuHelp;
        private MenuItem mnuPatientAccessHelp;
        private MenuItem mnuAboutPatientAccess;
        // Home
        private MenuItem mnuHome;

        private MenuItem mnuEditRecurringOutpatient;
        private MenuItem mnuFileLogOff;
        private MenuItem mnuFileLogOffExit;
        private MenuItem mnuUndo;
        private MenuItem mnuCut;
        private MenuItem mnuCopy;
        private MenuItem mnuPaste;
        private MenuItem menuItem1;
        private MenuItem mnuSelectAll;
        private MenuItem menuItem2;

        private StatusBarPanel sbpFacilityName;
        private StatusBarPanel sbpWorkstationId;
        private StatusBarPanel sbpVersionNumber;
        private StatusBarPanel sbpSpacer;
        private StatusBarPanel sbpAnotherSpacer;
        private StatusBarPanel sbpUserName;

        private bool i_ActivityStarted;
        private Timer i_AccountLockTimer;
        private readonly System.Windows.Forms.Timer imageRollOverTimer = new System.Windows.Forms.Timer();
        private volatile IAccount i_LockedAccount;
        private volatile IAccount i_LockedAccount2;
        private Control controlWithFocus;
        private Location i_LockedBedLocation;
        private IAccount i_SelectedAccount;
        private Activity i_CurrentActivity;

        #endregion

        #region Constants
        const int TIMESPAN = 200;

        const string MASKEDITTEXTBOX = "MaskedEditTextBox";
        const string TIMEOUTALERT = "TimeoutAlert";

        const bool SELECT_ALL = true,
                    NOT_SELECT_ALL = false;

        const string IMAGE_ADVANCED_DIRECTIVE_DISABLED = "AdvancedDirective_Disabled.ico",
                                                        IMAGE_ADVANCED_DIRECTIVE_ENABLED = "AdvancedDirective_Enabled.ico",
                                                        IMAGE_DRIVERS_LICENSE_DISABLED = "DriversLicense_Disabled.ico",
                                                        IMAGE_DRIVERS_LICENSE_ENABLED = "DriversLicense_Enabled.ico",
                                                        IMAGE_INSURANCE_CARD_DISABLED = "InsuranceCard_Disabled.ico",
                                                        IMAGE_INSURANCE_CARD_ENABLED = "InsuranceCard_Enabled.ico",
                                                        IMAGE_NPP_DISABLED = "NPP_Disabled.ico",
                                                        IMAGE_NPP_ENABLED = "NPP_Enabled.ico",
                                                        IMAGE_FUS_NOTES_DISABLED = "FUSNotes_Disabled.ico",
                                                        IMAGE_FUS_NOTES_ENABLED = "FUSNotes_Enabled.ico";
        #endregion

        private ToolBarButton toolBarSpacer2;
        private ToolBarButton btnFUSNotes;
        private ToolBarButton btnViewOnlinePreRegSupplementalInformation;
        private BackgroundWorker backgroundWorker;
        internal MenuItem mnuAdmin;
        private MenuItem mnuManageNewEmployers;
        private VIWebPreviousDocumentCnts previousVisitDocumentCnts;
        private MenuItem mnuQuickAccountCreation;
        private MenuItem mnuPAIWalkInOutpatientAccountCreation;
        private MenuItem menuItem3;
        private MenuItem menuItem4;
        private SupplementalInformationView supplementalInformationView = new SupplementalInformationView();

        private void mnuQuickAccountCeation_Click( object sender, EventArgs e )
        {

            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | QAC menu selected");
                ClearPanel();

                SetAccountViewType( false, true, false);

                QuickPreRegistrationView quickRegistrationView = new QuickPreRegistrationView
                                                                 {CurrentActivity = new QuickAccountCreationActivity()};
                quickRegistrationView.MPIV.CurrentActivity = quickRegistrationView.CurrentActivity;
                CurrentActivity = new QuickAccountCreationActivity();
                quickRegistrationView.ReturnToMainScreen += Cancel_Click;
                quickRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(quickRegistrationView);
            }
        }

        private void mnuPAIWalkInOUtpatientAccountCreation_Click(object sender, EventArgs e)
        {

            if (ConfirmedMenuClick())
            {
                BreadCrumbLogger.GetInstance.Log("Register | PAI walkin oupatient selected");
                ClearPanel();

                SetAccountViewType(false, false, true);

                QuickPreRegistrationView PAIWalkinRegistrationView = new QuickPreRegistrationView { CurrentActivity = new PAIWalkinOutpatientCreationActivity() };
                PAIWalkinRegistrationView.MPIV.CurrentActivity = PAIWalkinRegistrationView.CurrentActivity;
                CurrentActivity = new PAIWalkinOutpatientCreationActivity();
                PAIWalkinRegistrationView.ReturnToMainScreen += Cancel_Click;
                PAIWalkinRegistrationView.Dock = DockStyle.Fill;
                Panel.Controls.Add(PAIWalkinRegistrationView);
            }
        }
   
    }
}
