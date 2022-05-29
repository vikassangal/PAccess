using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;

namespace PatientAccess.UI.Registration
{
    public class NewbornPreRegistrationPresenter
    {
        #region Methods
        public void CreatePreNewBornAccountEventHandler( object sender, EventArgs e )
		{
			view.EnableAccountView();  

			if( e != null )                
			{
				if( ((LooseArgs)e).Context != null )
				{
					view.SelectedAccount = ((LooseArgs)e).Context as IAccount;
				}
			}

			if( backgroundWorker == null || ( !backgroundWorker.IsBusy ))
			{
				BeforeWork();
				backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
				backgroundWorker.DoWork +=
					DoWork;
				backgroundWorker.RunWorkerCompleted +=
					AfterWork;
				backgroundWorker.RunWorkerAsync();
			}
		}

        public void Dispose( bool disposing )
        {
            SearchEventAggregator.GetInstance().CreateNewBornAccountEvent -= CreatePreNewBornAccountEventHandler;
            SearchEventAggregator.GetInstance().AccountSelected -= OnEditAccount;
            if(disposing)
                CancelBackgroundWorker();
        }
        
        #endregion

        #region Construction and Finalization
        
        public NewbornPreRegistrationPresenter( INewbornPreRegistrationView nbView )
        {
            view = nbView;
            view.RegisterOnReturnToMainScreen( OnReturnToMainScreen );
            if ( !view.IsInDesignMode )
            {
                view.SetMasterPatientIndexViewActivity( view.CurrentActivity );
            }
            SearchEventAggregator.GetInstance().CreateNewBornAccountEvent += CreatePreNewBornAccountEventHandler;
            SearchEventAggregator.GetInstance().AccountSelected += OnEditAccount;
        }

        #endregion

        #region Private Methods
        private void BeforeWork()
        {
            view.SetCursor( Cursors.WaitCursor );

            if ( view.SelectedAccount != null )
            {
                if ( view.SelectedAccount.Activity != null )
                {
                    AccountView.GetInstance().ActiveContext = view.SelectedAccount.Activity.ContextDescription;
                }
                else
                {
                    if ( view.CurrentActivity != null )
                    {
                        AccountView.GetInstance().ActiveContext = view.CurrentActivity.ContextDescription;
                    }
                }
            }

            AccountView.GetInstance().ShowPanel();
        }
        private void DoWork( object sender, DoWorkEventArgs e )
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.

            Account newBornAccount = null;
            newBornAccount = AccountActivityService.SelectedAccountFor( view.SelectedAccount );

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: poll cancellationPending before doing time consuming work. 
            // this is not the best way to be polling, but the whole thing needs a bit of a rethink!
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            if ( newBornAccount != null && newBornAccount.Activity.GetType() == typeof( PreAdmitNewbornActivity ) )
            {
                //setting mother's account activity
                view.CurrentActivity.AppUser = User.GetCurrent();
                newBornAccount.Patient.IsNew = true;
                newBornAccount.IsNew = true;
                newBornAccount.KindOfVisit = VisitType.PreRegistration;
                newBornAccount.ActionsLoader = new ActionLoader( newBornAccount );
            }

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            //Nereid. Model
            view.ModelAccount = newBornAccount;

        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( view.IsDisposedOrDisposing )
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
                throw e.Error;
            }
            else
            {
                // success
                //DisplayAccountViewWith( Model as Account );
                DisplayAccountViewWith( view.ModelAccount );
            }

            // place post completion operations here...
            view.SetCursor( Cursors.Default );
            AccountView.GetInstance().HidePanel();
        }

        private void CancelBackgroundWorker()
        {
            // cancel the backgroundWorker  
            if ( backgroundWorker != null )
                backgroundWorker.CancelAsync();
        }

        private void DisplayAccountViewWith( Account anAccount )
        {
            view.accountView.OnCloseActivity += view.ReturnToMainScreen;
            view.accountView.OnEditAccount += OnEditAccount;
            view.accountView.OnRepeatActivity += OnRepeatActivity;

            view.accountView.Model = anAccount;
            view.accountView.UpdateView();
            view.accountView.Show();
        }

        private void OnEditAccount( object sender, EventArgs e )
        {

            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, e );
            CreatePreNewBornAccountEventHandler( this, e );
        }

        private void OnRepeatActivity( object sender, EventArgs e )
        {
            view.DisplayMasterPatientIndexView();
        }

        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if ( view.ReturnToMainScreen != null )
            {
                // Cancel any active background workers
                CancelBackgroundWorker();

                view.ReturnToMainScreen( sender, e );
            }
        }

        #endregion

        //public event EventHandler ReturnToMainScreen;
        #region Data Elements
        
        private readonly INewbornPreRegistrationView view;
        private BackgroundWorker backgroundWorker;
        
        #endregion
    }
}
