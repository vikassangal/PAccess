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
    ///   Summary description for TransferOutPatientToERPatView.
    /// </summary>
    public class OutPatientToEmergencyPatientView : ControlView
    {
        #region Events

        #endregion

        #region Event Handlers

        private void OutPatientToERPatStep3View_TabSelectedEvent(object sender, EventArgs e)
        {
            var index = (int) ((LooseArgs) e).Context;

            Controls[INDEX_OF_OutPatientToERPatStep1View].Hide();
            Controls[INDEX_OF_OutPatientToERPatStep2View].Hide();
            Controls[INDEX_OF_OutPatientToERPatStep3View].Hide();

            OutPatientToEmergencyPatientStep1View.Enabled = false;
            OutPatientToEmergencyPatientStep2View.Enabled = false;
            OutPatientToEmergencyPatientStep3View.Enabled = false;
            var physicianSearchForm = new PhysicianSearchFormView();

            switch (index)
            {
                case 0: // Transfer to

                    OutPatientToEmergencyPatientStep1View.Enabled = true;
                    Controls[INDEX_OF_OutPatientToERPatStep1View].Show();
                    Controls[INDEX_OF_OutPatientToERPatStep1View].Focus();
                    break;

                case 1: // Physicians

                    OutPatientToEmergencyPatientStep2View.Enabled = true;
                    Controls[INDEX_OF_OutPatientToERPatStep2View].Show();
                    Controls[INDEX_OF_OutPatientToERPatStep2View].Focus();
                    break;

                case 2: // Insurance Details

                    OutPatientToEmergencyPatientStep3View.Enabled = true;
                    Controls[INDEX_OF_OutPatientToERPatStep3View].Show();
                    Controls[INDEX_OF_OutPatientToERPatStep3View].Focus();
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
            OutPatientToEmergencyPatientStep2View.Enabled = true;
            Controls[INDEX_OF_OutPatientToERPatStep2View].Show();

            Cursor = Cursors.WaitCursor;
            physicianSearchForm.CallingObject = "VIEWDETAIL";
            physicianSearchForm.Model = Model as Account;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex =
                PhysicianSearchTabView.NON_STAFF_PHYSICIAN_TAB;
            physicianSearchForm.UpdateView();

            physicianSearchForm.ShowDialog(this);
            Cursor = Cursors.Default;

            OutPatientToEmergencyPatientStep2View.Model = Model as Account;
            OutPatientToEmergencyPatientStep2View.UpdateView();
        }

        private void BeforeWork()
        {
            storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            if (CurrentActivity.GetType() == typeof (TransferERToOutpatientActivity) ||
                CurrentActivity.GetType() == typeof (TransferOutpatientToERActivity))
            {
                AttachOutPatientToERPatView();
                OutPatientToEmergencyPatientStep1View.ShowPanel();
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
                    OutPatientToEmergencyPatientStep1View.HidePanel();
                    DisplayOutPatientToERPatView(Model as Account);
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
            Controls[INDEX_OF_OutPatientToERPatStep1View].Hide();
            OutPatientToEmergencyPatientStep1View.Enabled = false;
            OutPatientToEmergencyPatientStep2View.Enabled = true;
            Controls[INDEX_OF_OutPatientToERPatStep2View].Show();
            Controls[INDEX_OF_OutPatientToERPatStep2View].Focus();
        }

        private void Step2BackEventHandler(object sender, EventArgs e)
        {
            OutPatientToEmergencyPatientStep1View.Enabled = true;
            Controls[INDEX_OF_OutPatientToERPatStep1View].Show();
            Controls[INDEX_OF_OutPatientToERPatStep1View].Focus();
            Controls[INDEX_OF_OutPatientToERPatStep2View].Hide();
            OutPatientToEmergencyPatientStep2View.Enabled = false;
        }

        private void Step2NextEventHandler(object sender, EventArgs e)
        {
            Controls[INDEX_OF_OutPatientToERPatStep2View].Hide();
            OutPatientToEmergencyPatientStep2View.Enabled = false;
            OutPatientToEmergencyPatientStep3View.Enabled = true;
            Controls[INDEX_OF_OutPatientToERPatStep3View].Show();
            Controls[INDEX_OF_OutPatientToERPatStep3View].Focus();
        }

        private void Step3BackEventHandler(object sender, EventArgs e)
        {
            OutPatientToEmergencyPatientStep2View.Enabled = true;
            Controls[INDEX_OF_OutPatientToERPatStep2View].Show();
            Controls[INDEX_OF_OutPatientToERPatStep2View].Focus();
            Controls[INDEX_OF_OutPatientToERPatStep3View].Hide();
            OutPatientToEmergencyPatientStep3View.Enabled = false;
        }

        private void Step3FinishEventHandler(object sender, EventArgs e)
        {
            // Validate Physicians for Transfer Out to In Activity on 'Finish' click
            var isPhysicianValid = OutPatientToEmergencyPatientStep3View.PhysiciansValidated();

            if (isPhysicianValid != true)
            {
                OutPatientToEmergencyPatientStep3View.EnableFinishButton();
                Controls[INDEX_OF_OutPatientToERPatStep3View].Hide();

                OutPatientToEmergencyPatientStep2View.Enabled = true;
                Controls[INDEX_OF_OutPatientToERPatStep2View].Show();

                return;
            }

            if (!OutPatientToEmergencyPatientStep1View.HasUpdatedChiefComplaint())
            {
                var action = EmergencyPatientAndOutPatientStep3View.ShowChiefComplaintWarning();

                if (action == DialogResult.Yes)
                {
                    OutPatientToEmergencyPatientStep3View.EnableFinishButton();
                    Controls[INDEX_OF_OutPatientToERPatStep3View].Hide();

                    OutPatientToEmergencyPatientStep1View.Enabled = true;
                    Controls[INDEX_OF_OutPatientToERPatStep1View].Show();

                    OutPatientToEmergencyPatientStep1View.mtbChiefComplaint.Focus();
                }
                else
                {
                    WrapUpTransferOutPatientToERPat(OutPatientToEmergencyPatientStep1View.Model);
                }
            }
            else
            {
                WrapUpTransferOutPatientToERPat(OutPatientToEmergencyPatientStep1View.Model);
            }

            //Raise Activitycomplete event
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent(this,
                                                                             new LooseArgs(
                                                                                 OutPatientToEmergencyPatientStep1View.
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


        private void AttachOutPatientToERPatView()
        {
            OutPatientToEmergencyPatientStep1View = new OutPatientToEmergencyPatientStep1View
                {Name = "OutPatientToERPatStep1View", Dock = DockStyle.Fill};

            OutPatientToEmergencyPatientStep2View = new EmergencyPatientAndOutPatientStep2View
                {Name = "OutPatientToERPatStep2View", Dock = DockStyle.Fill};

            OutPatientToEmergencyPatientStep3View = new EmergencyPatientAndOutPatientStep3View
                {Name = "OutPatientToERPatStep3View", Dock = DockStyle.Fill};

            outPatientToEmergencyPatientConfirmView = new EmergencyPatientAndOutPatientConfirmView
                {Name = "OutPatientToERPatConfirmView", Dock = DockStyle.Fill};

            SuspendLayout();
            ClearControls();
            Controls.Add(OutPatientToEmergencyPatientStep1View);
            Controls.Add(OutPatientToEmergencyPatientStep2View);
            Controls.Add(OutPatientToEmergencyPatientStep3View);
            Controls.Add(outPatientToEmergencyPatientConfirmView);

            OutPatientToEmergencyPatientStep3View.TabSelectedEvent += OutPatientToERPatStep3View_TabSelectedEvent;

            ResumeLayout(false);
        }

        private void DisplayOutPatientToERPatView(Account anAccount)
        {
            OutPatientToEmergencyPatientStep1View.CancelButtonClicked += CancelEventHandler;
            OutPatientToEmergencyPatientStep1View.NextButtonClicked += Step1NextEventHandler;

            OutPatientToEmergencyPatientStep2View.CancelButtonClicked += CancelEventHandler;
            OutPatientToEmergencyPatientStep2View.BackButtonClicked += Step2BackEventHandler;
            OutPatientToEmergencyPatientStep2View.NextButtonClicked += Step2NextEventHandler;

            OutPatientToEmergencyPatientStep3View.CancelButtonClicked += CancelEventHandler;
            OutPatientToEmergencyPatientStep3View.BackButtonClicked += Step3BackEventHandler;
            OutPatientToEmergencyPatientStep3View.FinishButtonClicked += Step3FinishEventHandler;

            outPatientToEmergencyPatientConfirmView.CloseView += CloseViewEventHandler;
            outPatientToEmergencyPatientConfirmView.RepeatActivity += RepeatActivityEventHandler;
            outPatientToEmergencyPatientConfirmView.EditAccount += EditAccountEventHandler;

            OutPatientToEmergencyPatientStep1View.Model = anAccount;
            OutPatientToEmergencyPatientStep2View.Model = anAccount;
            OutPatientToEmergencyPatientStep3View.Model = anAccount;
            outPatientToEmergencyPatientConfirmView.Model = anAccount;

            OutPatientToEmergencyPatientStep2View.UpdateView();
            OutPatientToEmergencyPatientStep3View.UpdateView();

            OutPatientToEmergencyPatientStep2View.Enabled = false;
            OutPatientToEmergencyPatientStep3View.Enabled = false;
            outPatientToEmergencyPatientConfirmView.Enabled = false;

            OutPatientToEmergencyPatientStep1View.UpdateView();
            OutPatientToEmergencyPatientStep1View.Show();
        }

        private void BeforeSaveWork()
        {
            Cursor = Cursors.WaitCursor;
            OutPatientToEmergencyPatientStep3View.ShowPanel();

            OutPatientToEmergencyPatientStep2View.UpdateModel();
        }

        private void AfterSaveWork(object sender, RunWorkerCompletedEventArgs e)
        {
            OutPatientToEmergencyPatientStep3View.HidePanel();

            if (e.Cancelled)
            {
                OutPatientToEmergencyPatientStep3View.ShowPanel();
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
            }
            else
            {
                // success
                outPatientToEmergencyPatientConfirmView.Enabled = true;
                outPatientToEmergencyPatientConfirmView.UpdateView();

                OutPatientToEmergencyPatientStep3View.Enabled = false;
                Controls[INDEX_OF_OutPatientToERPatStep3View].Hide();

                Controls[INDEX_OF_OutPatientToERPatConfirmView].Show();
                Controls[INDEX_OF_OutPatientToERPatConfirmView].Focus();
            }

            Cursor = Cursors.Default;
        }

        private void DoWrapUpOutPatToEr(object sender, DoWorkEventArgs e)
        {
            OutPatientToEmergencyPatientStep1View.UpdateModel();
            OutPatientToEmergencyPatientStep3View.UpdateModel();

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

        private void WrapUpTransferOutPatientToERPat(Account anAccount)
        {
            wrapUpAccount = anAccount;

            if (backgroundWorker == null
                || (!backgroundWorker.IsBusy))
            {
                BeforeSaveWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoWrapUpOutPatToEr;
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
            // TransferOutPatientToERPatView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "OutPatientToEmergencyPatientView";
            this.Size = new System.Drawing.Size(16, 24);
        }

        #endregion
        
        #endregion

        #region Private Properties

        private TransferLocationDetailView DetailView { get; set; }

        private OutPatientToEmergencyPatientStep1View OutPatientToEmergencyPatientStep1View { get; set; }

        private EmergencyPatientAndOutPatientStep2View OutPatientToEmergencyPatientStep2View { get; set; }

        private EmergencyPatientAndOutPatientStep3View OutPatientToEmergencyPatientStep3View { get; set; }

        private EmergencyPatientAndOutPatientConfirmView outPatientToEmergencyPatientConfirmView { get; set; }


        private MasterPatientIndexView MasterPatientIndexView { get; set; }

        #endregion

        #region Construction and Finalization

        public OutPatientToEmergencyPatientView(Activity currentActivity)
        {
            CurrentActivity = currentActivity;
            SuspendLayout();

            InitializeComponent();

            if (CurrentActivity.GetType() == typeof (TransferOutpatientToERActivity))
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


                if (OutPatientToEmergencyPatientStep1View != null)
                {
                    OutPatientToEmergencyPatientStep1View.Dispose();
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

        private static readonly ILog c_log = LogManager.GetLogger(typeof (OutPatientToEmergencyPatientView));

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
            INDEX_OF_OutPatientToERPatStep1View = 0,
            INDEX_OF_OutPatientToERPatStep2View = 1,
            INDEX_OF_OutPatientToERPatStep3View = 2,
            INDEX_OF_OutPatientToERPatConfirmView = 3;

        #endregion
    }
}