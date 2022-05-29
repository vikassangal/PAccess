using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CancelInpatientStatusViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.PatientSearch;
using log4net;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.UI.DischargeViews
{
    public class DischargeView : ControlView
    {
        #region Event Handlers

        private void BeforeWork()
        {
            Cursor = Cursors.WaitCursor;

            if ( CurrentActivity.GetType().Equals( typeof( PreDischargeActivity ) )
                )
            {
                EnableDischargeIntentView();
                dischargeIntentView.ProgressPanel1.Visible = true;
                dischargeIntentView.ProgressPanel1.BringToFront();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( CancelInpatientDischargeActivity ) )
                )
            {
                EnableCancelInpatientDischargeView();
                cancelInpatientDischargeView.ProgressPanel1.Visible = true;
                cancelInpatientDischargeView.ProgressPanel1.BringToFront();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( CancelOutpatientDischargeActivity ) )
                )
            {
                EnableCancelOutpatientDischargeView();
                cancelOutpatientDischargeView.ProgressPanel1.Visible = true;
                cancelOutpatientDischargeView.ProgressPanel1.BringToFront();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( DischargeActivity ) )
                )
            {
                EnableDischargePatientView();
                dischargePatientView.ProgressPanel1.Visible = true;
                dischargePatientView.ProgressPanel1.BringToFront();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( EditDischargeDataActivity ) )
                )
            {
                EnableEditDischargeView();
                editDischargeView.ProgressPanel1.Visible = true;
                editDischargeView.ProgressPanel1.BringToFront();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( EditRecurringDischargeActivity ) )
                 )
            {
                EnableEditRecurringDischargeDateView();
                editRecurringDischargeDateView.ProgressPanel1.Visible = true;
                editRecurringDischargeDateView.ProgressPanel1.BringToFront();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( CancelInpatientStatusActivity ) )
               )
            {
                EnableCancelInpatientStatusView();
                cancelInpatientStatusView.ProgressPanel1.Visible = true;
                cancelInpatientStatusView.ProgressPanel1.BringToFront();
            }
        }

        private void DoWork( object sender, DoWorkEventArgs e )
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.

            if ( SelectedAccount != null )
            {
                // Poll the cancellationPending property, if true set e.Cancel to true and return.
                // Rationale: poll cancellationPending before doing time consuming work. 
                // this is not the best way to be polling, but the whole thing needs a bit of a rethink!
                if ( backgroundWorker.CancellationPending )
                {
                    e.Cancel = true;
                    return;
                }
                if ( CurrentActivity.GetType().Equals( typeof( PreDischargeActivity ) ) )
                {
                    DisplayDischargeIntentView();
                }
                else if ( CurrentActivity.GetType().Equals( typeof( CancelInpatientDischargeActivity ) ) )
                {
                    DisplayCancelInpatientDischargeView();
                }
                else if ( CurrentActivity.GetType().Equals( typeof( CancelOutpatientDischargeActivity ) ) )
                {
                    DisplayCancelOutpatientDischargeView();
                }
                else if ( CurrentActivity.GetType().Equals( typeof( DischargeActivity ) ) )
                {
                    DisplayDischargePatientView();
                }
                else if ( CurrentActivity.GetType().Equals( typeof( EditDischargeDataActivity ) ) )
                {
                    DisplayEditDischargeView();
                }
                else if ( CurrentActivity.GetType().Equals( typeof( EditRecurringDischargeActivity ) ) )
                {
                    DisplayEditRecurringDischargeDateView();
                }
                else if ( CurrentActivity.GetType().Equals( typeof( CancelInpatientStatusActivity ) ) )
                {
                    DisplayCancelInpatientStatusView();
                }

                // Poll the cancellationPending property, if true set e.Cancel to true and return.
                // Rationale: poll cancellationPending before doing time consuming work. 
                // this is not the best way to be polling, but the whole thing needs a bit of a rethink!
                if ( backgroundWorker.CancellationPending )
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        // Sanjeev Kumar: Refactored the logic of this method so that we can see clearly what's going on.
        // Refactored existing logic into a private method called, RptActivityEditAccountEventWireRegistering
        // that takes no arguments and operates only upon class fields. 
        // This is then called in the success part of the conditional construct in the AfterWork. 
        // We then have a cleaner AfterWork method whose flow we can clearly see.
        //
        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            if ( e.Cancelled )
            {
                // user cancelled
                // Due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
            }
            else
            {
                // success
                RptActivityEditAccountEventWireRegistering();
            }

            // place post completion operations here...
            Cursor = Cursors.Default;
        }

        // Sanjeev Kumar: Refactored existing logic from AfterWork into a private method called,         
        // RptActivityEditAccountEventWireRegistering()
        // that takes no arguments and operates only upon class fields.
        //
        private void RptActivityEditAccountEventWireRegistering()
        {
            if ( CurrentActivity.GetType().Equals( typeof( PreDischargeActivity ) ) )
            {
                dischargeIntentView.RepeatActivity += RepeatActivityEventHandler;
                dischargeIntentView.EditAccount += EditAccountEventHandler;

                dischargeIntentView.Model = Model as Account;

                dischargeIntentView.UpdateView();
                dischargeIntentView.Show();

                dischargeIntentView.ProgressPanel1.Visible = false;
            }
            else if ( CurrentActivity.GetType().Equals( typeof( CancelInpatientDischargeActivity ) ) )
            {
                cancelInpatientDischargeView.RepeatActivity += RepeatActivityEventHandler;
                cancelInpatientDischargeView.EditAccount += EditAccountEventHandler;

                cancelInpatientDischargeView.Model = Model as Account;

                cancelInpatientDischargeView.UpdateView();
                cancelInpatientDischargeView.Show();

                cancelInpatientDischargeView.ProgressPanel1.Visible = false;
            }
            else if ( CurrentActivity.GetType().Equals( typeof( CancelOutpatientDischargeActivity ) ) )
            {
                cancelOutpatientDischargeView.RepeatActivity += RepeatActivityEventHandler;
                cancelOutpatientDischargeView.EditAccount += EditAccountEventHandler;

                cancelOutpatientDischargeView.Model = Model as Account;

                cancelOutpatientDischargeView.UpdateView();
                cancelOutpatientDischargeView.Show();

                cancelOutpatientDischargeView.ProgressPanel1.Visible = false;
            }
            else if ( CurrentActivity.GetType().Equals( typeof( DischargeActivity ) ) )
            {
                dischargePatientView.RepeatActivity += RepeatActivityEventHandler;
                dischargePatientView.EditAccount += EditAccountEventHandler;

                dischargePatientView.Model = Model as Account;

                dischargePatientView.UpdateView();
                dischargePatientView.Show();

                dischargePatientView.ProgressPanel1.Visible = false;
                dischargePatientView.ProgressPanel1.SendToBack();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( EditDischargeDataActivity ) ) )
            {
                editDischargeView.RepeatActivity += RepeatActivityEventHandler;
                editDischargeView.EditAccount += EditAccountEventHandler;

                editDischargeView.Model = Model as Account;

                editDischargeView.UpdateView();
                editDischargeView.Show();

                editDischargeView.ProgressPanel1.Visible = false;
            }
            else if ( CurrentActivity.GetType().Equals( typeof( EditRecurringDischargeActivity ) ) )
            {
                editRecurringDischargeDateView.RepeatActivity += RepeatActivityEventHandler;
                editRecurringDischargeDateView.EditAccount += EditAccountEventHandler;

                editRecurringDischargeDateView.Model = Model as Account;

                editRecurringDischargeDateView.UpdateView();
                editRecurringDischargeDateView.Show();

                editRecurringDischargeDateView.ProgressPanel1.Visible = false;
            }
            else if ( CurrentActivity.GetType().Equals( typeof( CancelInpatientStatusActivity ) ) )
            {
                cancelInpatientStatusView.RepeatActivity += RepeatActivityEventHandler;
                cancelInpatientStatusView.EditAccount += EditAccountEventHandler;

                cancelInpatientStatusView.Model = Model as Account;

                cancelInpatientStatusView.UpdateView();
                cancelInpatientStatusView.Show();

                cancelInpatientStatusView.ProgressPanel1.Visible = false;
            }
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {
            SelectedAccount = ( ( LooseArgs )e ).Context as AccountProxy;

            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void RepeatActivityEventHandler( object sender, EventArgs e )
        {
            DisplayMasterPatientIndexView();
        }

        private void EditAccountEventHandler( object sender, EventArgs e )
        {
            Cursor storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                if ( e != null )
                {
                    Account realAccount = ( Account )( ( LooseArgs )e ).Context;

                    if ( ( realAccount.KindOfVisit != null &&
                          realAccount.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT &&
                          realAccount.FinancialClass != null &&
                          realAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE ) )
                    {
                        ViewFactory.Instance.CreateView<PatientAccessView>().LoadEditPreMSEView();

                        realAccount.Activity.AssociatedActivityType = typeof( PreMSERegisterActivity );

                        LooseArgs args = new LooseArgs( realAccount );

                        SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );
                        ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                    }
                    else
                        if (realAccount.IsUrgentCarePreMse)
                        {
                            ViewFactory.Instance.CreateView<PatientAccessView>().LoadEditUCCPreMSEView();

                            realAccount.Activity.AssociatedActivityType = typeof(UCCPreMSERegistrationActivity);

                            LooseArgs args = new LooseArgs(realAccount);

                            SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent(this, args);
                            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this, args);
                        }
                    else
                    {
                        ViewFactory.Instance.CreateView<PatientAccessView>().ActivateTab( null, realAccount );
                    }
                }
            }
            finally
            {
                Cursor = storedCursor;
            }
        }
        #endregion

        #region Methods
       
        #endregion

        #region Properties
        public ControlView ParentControl
        {
            get
            {
                return i_ParentControl;
            }
            set
            {
                i_ParentControl = value;
            }
        }

        private Activity CurrentActivity
        {
            get
            {
                return i_CurrentActivity;
            }
            // next lines are temporary; once the dischargeView has been broken into each of its
            // controllers, this set accessor can be removed.
            set
            {
                // if current activity not initialized, default to DischargeActivity.
                // this will be immediately overlayed by the 'set' activity.
                if ( i_CurrentActivity == null )
                {
                    i_CurrentActivity = new DischargeActivity();
                }
                i_CurrentActivity = value;
            }
        }
        #endregion

        #region Private Methods

        private void EnableDischargeIntentView()
        {
            ClearControls();
            dischargeIntentView = new DischargeIntentView();

            SuspendLayout();
            dischargeIntentView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( dischargeIntentView );
        }

        private void DisplayDischargeIntentView()
        {
            SelectedAccount.Activity = new PreDischargeActivity();
            Account realAccount = AccountActivityService.SelectedAccountFor( SelectedAccount );
            Model = realAccount;
        }

        private void EnableDischargePatientView()
        {
            ClearControls();
            dischargePatientView = new DischargePatientView();

            SuspendLayout();
            dischargePatientView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( dischargePatientView );
        }

        private void DisplayDischargePatientView()
        {
            SelectedAccount.Activity = CurrentActivity;

            // Defect 9067 fix - Use SelectedAccountFor() as against AccountProxy.AsAccount() 
            // method, since we need to read and send back the Operating Physician information 
            // that was getting wiped during patient discharge.
            Account realAccount = AccountActivityService.SelectedAccountFor( SelectedAccount );

            Model = realAccount;
        }

        private void EnableCancelInpatientDischargeView()
        {
            ClearControls();
            cancelInpatientDischargeView = new CancelInpatientDischargeView();

            SuspendLayout();
            cancelInpatientDischargeView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( cancelInpatientDischargeView );
        }

        private void DisplayCancelInpatientDischargeView()
        {
            SelectedAccount.Activity = new CancelInpatientDischargeActivity();
            Account realAccount = SelectedAccount.AsAccount();
            Model = realAccount;
        }

        private void EnableCancelOutpatientDischargeView()
        {
            ClearControls();
            cancelOutpatientDischargeView = new CancelOutpatientDischargeView();

            SuspendLayout();
            cancelOutpatientDischargeView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( cancelOutpatientDischargeView );
        }

        private void DisplayCancelOutpatientDischargeView()
        {
            SelectedAccount.Activity = new CancelOutpatientDischargeActivity();
            Account realAccount = SelectedAccount.AsAccount();
            Model = realAccount;
        }

        private void EnableEditDischargeView()
        {
            ClearControls();
            editDischargeView = new EditDischargeView();

            SuspendLayout();
            editDischargeView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( editDischargeView );
        }

        private void DisplayEditDischargeView()
        {
            SelectedAccount.Activity = new EditDischargeDataActivity();

            // Defect 11025 fix - Use SelectedAccountFor() as against AccountProxy.AsAccount() 
            // method, since we need to read and send back the Operating Physician information 
            // that was getting wiped during patient discharge.
            Account realAccount = AccountActivityService.SelectedAccountFor( SelectedAccount );

            Model = realAccount;
        }

        private void EnableEditRecurringDischargeDateView()
        {
            ClearControls();
            editRecurringDischargeDateView = new EditRecurringDischargeDateView();

            SuspendLayout();
            editRecurringDischargeDateView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( editRecurringDischargeDateView );
        }

        private void DisplayEditRecurringDischargeDateView()
        {
            SelectedAccount.Activity = new EditRecurringDischargeActivity();
            Account realAccount = SelectedAccount.AsAccount();
            Model = realAccount;
        }

        private void EnableCancelInpatientStatusView()
        {
            ClearControls();
            cancelInpatientStatusView = new CancelInpatientStatusView();

            SuspendLayout();
            cancelInpatientStatusView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( cancelInpatientStatusView );
        }

        private void DisplayCancelInpatientStatusView()
        {
            SelectedAccount.Activity = new CancelInpatientStatusActivity();
            Account realAccount = AccountActivityService.SelectedAccountFor( SelectedAccount );
            Model = realAccount;
        }

        private void DisplayMasterPatientIndexView()
        {
            ClearControls();
            masterPatientIndexView = new MasterPatientIndexView
                { CurrentActivity = CurrentActivity, Dock = DockStyle.Fill };
            Controls.Add( masterPatientIndexView );
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
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
            this.SuspendLayout();
            // 
            // masterPatientIndexView
            // 
            this.masterPatientIndexView.Location = new System.Drawing.Point( 0, 0 );
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size( 1024, 512 );
            this.masterPatientIndexView.Dock = DockStyle.Fill;
            this.masterPatientIndexView.TabIndex = 0;
            // 
            // DischargeView
            // 
            this.Controls.Add( this.masterPatientIndexView );
            this.Name = "DischargeView";
            this.Size = new System.Drawing.Size( 1024, 512 );
            this.ResumeLayout( false );
        }
        #endregion

        #region Construction and Finalization

        public DischargeView( Activity inActivity )
        {
            CurrentActivity = inActivity;

            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;

            if ( disposing )
            {
                // cancel the backgroundWorker if it's still alive.
                // need a more appropriate place to enter such backgroundworker clean up code
                if ( backgroundWorker != null )
                    backgroundWorker.CancelAsync();

                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( DischargeView ) );

        private AccountProxy SelectedAccount;
        private ControlView i_ParentControl;
        DischargeIntentView dischargeIntentView;
        CancelInpatientDischargeView cancelInpatientDischargeView;
        CancelOutpatientDischargeView cancelOutpatientDischargeView;
        DischargePatientView dischargePatientView;
        EditDischargeView editDischargeView;

        EditRecurringDischargeDateView editRecurringDischargeDateView;
        CancelInpatientStatusView cancelInpatientStatusView;
        private Container components = null;
        private MasterPatientIndexView masterPatientIndexView;
        private Activity i_CurrentActivity;
        private BackgroundWorker backgroundWorker;
        #endregion

        #region Constants
        #endregion
    }
}
