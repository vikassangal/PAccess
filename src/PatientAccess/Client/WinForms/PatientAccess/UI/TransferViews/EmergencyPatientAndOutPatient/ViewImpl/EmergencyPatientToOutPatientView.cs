using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PhysicianSearchViews;
using log4net;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient.ViewImpl
{
    /// <summary>
    ///   Summary description for TransferERPatToOutPatView.
    /// </summary>
    public class EmergencyPatientToOutPatientView : ControlView
    {
        #region Events

        #endregion

        #region Event Handlers

        private void ERPatToOutPatStep3View_TabSelectedEvent(object sender, EventArgs e)
        {
            var index = (int) ((LooseArgs) e).Context;

            Controls[INDEX_OF_ERPatToOutPatStep1View].Hide();
            Controls[INDEX_OF_ERPatToOutPatStep2View].Hide();
            Controls[INDEX_OF_ERPatToOutPatStep3View].Hide();

            EmergencyPatientToOutPatientStep1View.Enabled = false;
            EmergencyPatientToOutPatientStep2View.Enabled = false;
            EmergencyPatientToOutPatientStep3View.Enabled = false;
            var physicianSearchForm = new PhysicianSearchFormView();

            switch (index)
            {
                case 0: // Transfer to

                    EmergencyPatientToOutPatientStep1View.Enabled = true;
                    Controls[INDEX_OF_ERPatToOutPatStep1View].Show();
                    Controls[INDEX_OF_ERPatToOutPatStep1View].Focus();
                    break;

                case 1: // Physicians

                    EmergencyPatientToOutPatientStep2View.Enabled = true;
                    Controls[INDEX_OF_ERPatToOutPatStep2View].Show();
                    Controls[INDEX_OF_ERPatToOutPatStep2View].Focus();
                    break;

                case 2: // Insurance Details

                    EmergencyPatientToOutPatientStep3View.Enabled = true;
                    Controls[INDEX_OF_ERPatToOutPatStep3View].Show();
                    Controls[INDEX_OF_ERPatToOutPatStep3View].Focus();
                    break;

                case 7: // Referring NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.REFERRING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage(physicianSearchForm);
                    break;

                case 8: // Attending NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ATTENDING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage(physicianSearchForm);
                    break;

                case 9: // Admitting NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ADMITTING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage(physicianSearchForm);
                    break;

                case 10: // Operating NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.OPERATING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage(physicianSearchForm);
                    break;

                case 11: // PrimaryCare NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.PRIMARYCARE_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage(physicianSearchForm);
                    break;
            }
        }

        private void DisplayNonStaffPhysicianDialogPage(PhysicianSearchFormView physicianSearchForm)
        {
            EmergencyPatientToOutPatientStep2View.Enabled = true;
            Controls[INDEX_OF_ERPatToOutPatStep2View].Show();

            Cursor = Cursors.WaitCursor;
            physicianSearchForm.CallingObject = "VIEWDETAIL";
            physicianSearchForm.Model = Model as Account;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex =
                PhysicianSearchTabView.NON_STAFF_PHYSICIAN_TAB;
            physicianSearchForm.UpdateView();

            physicianSearchForm.ShowDialog(this);
            Cursor = Cursors.Default;

            EmergencyPatientToOutPatientStep2View.Model = Model as Account;
            EmergencyPatientToOutPatientStep2View.UpdateView();
        }

        private void BeforeWork()
        {
            storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            if (CurrentActivity.GetType() == typeof (TransferERToOutpatientActivity) ||
                CurrentActivity.GetType() == typeof (TransferOutpatientToERActivity))
            {
                AttachERPatToOutPatView();
                EmergencyPatientToOutPatientStep1View.ShowPanel();
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;

            var proxy = SelectedAccount as AccountProxy;
            Account account = null;
            if (proxy != null)
            {
                // TLG 05/21/2007 replace proxy.AsAccount with streamlined call

                account = AccountActivityService.SelectedAccountFor(proxy);

                account.Activity = CurrentActivity;

                // poll CancellationPending and set e.Cancel to true and return 
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Model = account;

            // poll CancellationPending and set e.Cancel to true and return 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed || Disposing)
                return;

            if (e.Cancelled)
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success
                if (CurrentActivity.GetType() == typeof (TransferERToOutpatientActivity) ||
                    CurrentActivity.GetType() == typeof (TransferOutpatientToERActivity))
                {
                    EmergencyPatientToOutPatientStep1View.HidePanel();
                    DisplayERPatToOutPatView(Model as Account);
                }
            }

            Cursor = storedCursor;
        }

        private void AccountSelectedEventHandler(object sender, EventArgs e)
        {
            SelectedAccount = ((LooseArgs) e).Context as AccountProxy;

            if (backgroundWorker == null ||
                !backgroundWorker.IsBusy)
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void CloseViewEventHandler(object sender, EventArgs e)
        {
            //Raise ActivityComplete event
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent(this, new LooseArgs(Model));

            CancelBackgroundWorker();

            // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
            DisableIconsAndMenuOptions();

            Dispose();
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen(this, null);
        }

        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if (backgroundWorker != null)
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void RepeatActivityEventHandler(object sender, EventArgs e)
        {
            DisplayMasterPatientIndexView();
        }

        private static void EditAccountEventHandler(object sender, EventArgs e)
        {
            var realAccount = (Account) ((LooseArgs) e).Context;
            ViewFactory.Instance.CreateView<PatientAccessView>().ActivateTab(null, realAccount);
        }

        private void CancelEventHandler(object sender, EventArgs e)
        {
            ClearControls();

            // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
            DisableIconsAndMenuOptions();

            Dispose();
        }

        private void Step1NextEventHandler(object sender, EventArgs e)
        {
            Controls[INDEX_OF_ERPatToOutPatStep1View].Hide();
            EmergencyPatientToOutPatientStep1View.Enabled = false;
            EmergencyPatientToOutPatientStep2View.Enabled = true;
            Controls[INDEX_OF_ERPatToOutPatStep2View].Show();
            Controls[INDEX_OF_ERPatToOutPatStep2View].Focus();
        }

        private void Step2BackEventHandler(object sender, EventArgs e)
        {
            EmergencyPatientToOutPatientStep1View.Enabled = true;
            Controls[INDEX_OF_ERPatToOutPatStep1View].Show();
            Controls[INDEX_OF_ERPatToOutPatStep1View].Focus();
            Controls[INDEX_OF_ERPatToOutPatStep2View].Hide();
            EmergencyPatientToOutPatientStep2View.Enabled = false;
        }

        private void Step2NextEventHandler(object sender, EventArgs e)
        {
            Controls[INDEX_OF_ERPatToOutPatStep2View].Hide();
            EmergencyPatientToOutPatientStep2View.Enabled = false;
            EmergencyPatientToOutPatientStep3View.Enabled = true;
            Controls[INDEX_OF_ERPatToOutPatStep3View].Show();
            Controls[INDEX_OF_ERPatToOutPatStep3View].Focus();
        }

        private void Step3BackEventHandler(object sender, EventArgs e)
        {
            EmergencyPatientToOutPatientStep2View.Enabled = true;
            Controls[INDEX_OF_ERPatToOutPatStep2View].Show();
            Controls[INDEX_OF_ERPatToOutPatStep2View].Focus();
            Controls[INDEX_OF_ERPatToOutPatStep3View].Hide();
            EmergencyPatientToOutPatientStep3View.Enabled = false;
        }

        private void Step3FinishEventHandler(object sender, EventArgs e)
        {
            // Validate Physicians for Transfer Out to In Activity on 'Finish' click
            var isPhysicianValid = EmergencyPatientToOutPatientStep3View.PhysiciansValidated();

            if (isPhysicianValid != true)
            {
                EmergencyPatientToOutPatientStep3View.EnableFinishButton();
                Controls[INDEX_OF_ERPatToOutPatStep3View].Hide();

                EmergencyPatientToOutPatientStep2View.Enabled = true;
                Controls[INDEX_OF_ERPatToOutPatStep2View].Show();

                return;
            }

            if (!EmergencyPatientToOutPatientStep1View.HasUpdatedChiefComplaint())
            {
                var action = EmergencyPatientAndOutPatientStep3View.ShowChiefComplaintWarning();

                if (action == DialogResult.Yes)
                {
                    EmergencyPatientToOutPatientStep3View.EnableFinishButton();
                    Controls[INDEX_OF_ERPatToOutPatStep3View].Hide();

                    EmergencyPatientToOutPatientStep1View.Enabled = true;
                    Controls[INDEX_OF_ERPatToOutPatStep1View].Show();

                    EmergencyPatientToOutPatientStep1View.mtbChiefComplaint.Focus();
                }
                else
                {
                    WrapUpTransferERPatToOutPat(EmergencyPatientToOutPatientStep1View.Model);
                }
            }
            else
            {
                WrapUpTransferERPatToOutPat(EmergencyPatientToOutPatientStep1View.Model);
            }

            //Raise Activitycomplete event
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent(this,
                                                                             new LooseArgs(
                                                                                 EmergencyPatientToOutPatientStep1View.
                                                                                     Model));
        }

        #endregion

        #region public Methods

        #endregion

        #region Public Properties

        private Activity CurrentActivity { get; // next lines are temporary; once the dischargeView has been broken into each of its
            // controllers, this set accessor can be removed.
            set; }

        #endregion

        #region Private Methods

        private static bool checkForError()
        {
            return RuleEngine.GetInstance().AccountHasFailedError();
        }

        protected override void WndProc(ref Message m)
        {
            const uint WM_NOTIFY = 0x004E;
            const uint TCN_FIRST = 0xFFFFFDDA;
            const uint TCN_SELCHANGING = TCN_FIRST - 2;

            base.WndProc(ref m);

            switch ((uint) m.Msg)
            {
                case WM_NOTIFY:
                    {
                        var nm = new NMHDR();
                        nm.hwndFrom = IntPtr.Zero;
                        nm.idFrom = 0;
                        nm.code = 0;

                        var idCtrl = (int) m.WParam;
                        var nmh = (NMHDR) m.GetLParam(typeof (NMHDR));

                        if (nmh.code == TCN_SELCHANGING)
                        {
                            bool rc = checkForError();
                            int irc = 0;
                            if (rc)
                            {
                                irc = 1;
                            }

                            Convert.ToInt32(rc);
                            m.Result = (IntPtr) irc;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private static void DisableIconsAndMenuOptions()
        {
            // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
            ViewFactory.Instance.CreateView<PatientAccessView>().Model = null;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions(false);
            ViewFactory.Instance.CreateView<PatientAccessView>().DisablePreviousDocumentOptions();
        }

        private void DisplayMasterPatientIndexView()
        {
            ClearControls();
            DisableIconsAndMenuOptions();
            MasterPatientIndexView = new MasterPatientIndexView();

            if (!IsInDesignMode)
            {
                MasterPatientIndexView.CurrentActivity = CurrentActivity;
            }

            MasterPatientIndexView.Dock = DockStyle.Fill;
            Controls.Add(MasterPatientIndexView);
            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;
        }

        private void AttachMasterPatientIndexView()
        {
            if (MasterPatientIndexView == null)
            {
                MasterPatientIndexView = new MasterPatientIndexView();
                if (!IsInDesignMode)
                {
                    MasterPatientIndexView.CurrentActivity = CurrentActivity;
                }
                MasterPatientIndexView.Model = null;
                MasterPatientIndexView.Name = "masterPatientIndexView";
                MasterPatientIndexView.Dock = DockStyle.Fill;
            }

            SuspendLayout();
            ClearControls();
            Controls.Add(MasterPatientIndexView);

            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;

            ResumeLayout(false);
        }

        private void ClearControls()
        {
            if (Controls == null)
            {
                return;
            }

            foreach (Control control in Controls)
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
            Controls.Clear();
        }


        private void AttachERPatToOutPatView()
        {
            EmergencyPatientToOutPatientStep1View = new EmergencyPatientToOutPatientStep1View
                {Name = "ERPatToOutPatStep1View", Dock = DockStyle.Fill};

            EmergencyPatientToOutPatientStep2View = new EmergencyPatientAndOutPatientStep2View
                {Name = "ERPatToOutPatStep2View", Dock = DockStyle.Fill};

            EmergencyPatientToOutPatientStep3View = new EmergencyPatientAndOutPatientStep3View
                {Name = "ERPatToOutPatStep3View", Dock = DockStyle.Fill};

            emergencyPatientToOutPatientConfirmView = new EmergencyPatientAndOutPatientConfirmView
                {Name = "ERPatToOutPatConfirmView", Dock = DockStyle.Fill};

            SuspendLayout();
            ClearControls();
            Controls.Add(EmergencyPatientToOutPatientStep1View);
            Controls.Add(EmergencyPatientToOutPatientStep2View);
            Controls.Add(EmergencyPatientToOutPatientStep3View);
            Controls.Add(emergencyPatientToOutPatientConfirmView);

            EmergencyPatientToOutPatientStep3View.TabSelectedEvent += ERPatToOutPatStep3View_TabSelectedEvent;

            ResumeLayout(false);
        }

        private void DisplayERPatToOutPatView(Account anAccount)
        {
            EmergencyPatientToOutPatientStep1View.CancelButtonClicked += CancelEventHandler;
            EmergencyPatientToOutPatientStep1View.NextButtonClicked += Step1NextEventHandler;

            EmergencyPatientToOutPatientStep2View.CancelButtonClicked += CancelEventHandler;
            EmergencyPatientToOutPatientStep2View.BackButtonClicked += Step2BackEventHandler;
            EmergencyPatientToOutPatientStep2View.NextButtonClicked += Step2NextEventHandler;

            EmergencyPatientToOutPatientStep3View.CancelButtonClicked += CancelEventHandler;
            EmergencyPatientToOutPatientStep3View.BackButtonClicked += Step3BackEventHandler;
            EmergencyPatientToOutPatientStep3View.FinishButtonClicked += Step3FinishEventHandler;

            emergencyPatientToOutPatientConfirmView.CloseView += CloseViewEventHandler;
            emergencyPatientToOutPatientConfirmView.RepeatActivity += RepeatActivityEventHandler;
            emergencyPatientToOutPatientConfirmView.EditAccount += EditAccountEventHandler;

            EmergencyPatientToOutPatientStep1View.Model = anAccount;
            EmergencyPatientToOutPatientStep2View.Model = anAccount;
            EmergencyPatientToOutPatientStep3View.Model = anAccount;
            emergencyPatientToOutPatientConfirmView.Model = anAccount;

            EmergencyPatientToOutPatientStep2View.UpdateView();
            EmergencyPatientToOutPatientStep3View.UpdateView();

            EmergencyPatientToOutPatientStep2View.Enabled = false;
            EmergencyPatientToOutPatientStep3View.Enabled = false;
            emergencyPatientToOutPatientConfirmView.Enabled = false;

            EmergencyPatientToOutPatientStep1View.UpdateView();
            EmergencyPatientToOutPatientStep1View.Show();
        }

        private void BeforeSaveWork()
        {
            Cursor = Cursors.WaitCursor;
            EmergencyPatientToOutPatientStep3View.ShowPanel();

            EmergencyPatientToOutPatientStep2View.UpdateModel();
        }

        private void AfterSaveWork(object sender, RunWorkerCompletedEventArgs e)
        {
            EmergencyPatientToOutPatientStep3View.HidePanel();

            if (e.Cancelled)
            {
                EmergencyPatientToOutPatientStep3View.ShowPanel();
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
            }
            else
            {
                // success
                emergencyPatientToOutPatientConfirmView.Enabled = true;
                emergencyPatientToOutPatientConfirmView.UpdateView();

                EmergencyPatientToOutPatientStep3View.Enabled = false;
                Controls[INDEX_OF_ERPatToOutPatStep3View].Hide();

                Controls[INDEX_OF_ERPatToOutPatConfirmView].Show();
                Controls[INDEX_OF_ERPatToOutPatConfirmView].Focus();
            }

            Cursor = Cursors.Default;
        }

        private void DoWrapUpERToOut(object sender, DoWorkEventArgs e)
        {
            EmergencyPatientToOutPatientStep1View.UpdateModel();
            EmergencyPatientToOutPatientStep3View.UpdateModel();

            // poll CancellationPending and set e.Cancel to true and return 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
 
            TransferService.QueueTransfer(wrapUpAccount);

            // poll CancellationPending and set e.Cancel to true and return 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void WrapUpTransferERPatToOutPat(Account anAccount)
        {
            wrapUpAccount = anAccount;

            if (backgroundWorker == null
                || (!backgroundWorker.IsBusy))
            {
                BeforeSaveWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoWrapUpERToOut;
                backgroundWorker.RunWorkerCompleted += AfterSaveWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // TransferERPatToOutPatView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "EmergencyPatientToOutPatientView";
            this.Size = new System.Drawing.Size(16, 24);
        }

        #endregion

        

        #endregion

        #region Private Properties

        private TransferLocationDetailView DetailView { get; set; }

        private EmergencyPatientToOutPatientStep1View EmergencyPatientToOutPatientStep1View { get; set; }

        private EmergencyPatientAndOutPatientStep2View EmergencyPatientToOutPatientStep2View { get; set; }

        private EmergencyPatientAndOutPatientStep3View EmergencyPatientToOutPatientStep3View { get; set; }

        private EmergencyPatientAndOutPatientConfirmView emergencyPatientToOutPatientConfirmView { get; set; }


        private MasterPatientIndexView MasterPatientIndexView { get; set; }

        #endregion

        #region Construction and Finalization

        public EmergencyPatientToOutPatientView(Activity currentActivity)
        {
            CurrentActivity = currentActivity;
            SuspendLayout();

            InitializeComponent();

            if (CurrentActivity.GetType() == typeof (TransferERToOutpatientActivity) ||
                CurrentActivity.GetType() == typeof (TransferOutpatientToERActivity))
            {
                AttachMasterPatientIndexView();
            }

            ResumeLayout(false);
            EnableThemesOn(this);
        }

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;

            if (disposing)
            {
                if (DetailView != null)
                {
                    DetailView.Dispose();
                }


                if (EmergencyPatientToOutPatientStep1View != null)
                {
                    EmergencyPatientToOutPatientStep1View.Dispose();
                }


                if (components != null)
                {
                    components.Dispose();
                }

                // cancel the background worker here...
                CancelBackgroundWorker();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Data Elements

        private static readonly ILog c_log = LogManager.GetLogger(typeof (EmergencyPatientToOutPatientView));

        /// <summary>
        ///   Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private IAccount SelectedAccount;
        private BackgroundWorker backgroundWorker;

        private Cursor storedCursor;
        private Account wrapUpAccount;

        #endregion

        #region Constants

        private const int
            INDEX_OF_ERPatToOutPatStep1View = 0,
            INDEX_OF_ERPatToOutPatStep2View = 1,
            INDEX_OF_ERPatToOutPatStep3View = 2,
            INDEX_OF_ERPatToOutPatConfirmView = 3;

        #endregion
    }
}