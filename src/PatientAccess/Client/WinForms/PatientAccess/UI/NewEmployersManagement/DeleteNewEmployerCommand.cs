using PatientAccess.Domain;

namespace PatientAccess.UI.NewEmployersManagement
{
    internal sealed class DeleteNewEmployerCommand : ICommand
    {
        private NewEmployerEntry _employerEntry;
        private NewEmployersManagementPresenter _presenter;


        public DeleteNewEmployerCommand(NewEmployersManagementPresenter presenter, NewEmployerEntry employerEntry)
        {
            this.Presenter = presenter;
            this.EmployerEntry = employerEntry;
        }

        private NewEmployersManagementPresenter Presenter
        {
            get
            {
                return _presenter;
            }
            set
            {
                _presenter = value;
            }
        }

        private NewEmployerEntry EmployerEntry
        {
            get
            {
                return this._employerEntry;
            }
            set
            {
                this._employerEntry = value;
            }
        }

        #region ICommand Members

        public void ExecuteUIAction()
        {
            this.Presenter.ClearNewEmployerDetailFields();
            this.Presenter.NewEmployers.Remove(this.EmployerEntry);
            this.Presenter.UpdateNewEmployersViewWithoutAnySelection();
        }


        public void UndoUIAction()
        {
            this.Presenter.AddNewEmployer(this.EmployerEntry);
            this.Presenter.SelectNewEmployerAndSearchMasterEmployerList(this.EmployerEntry);
            this.Presenter.NewEmployerListView.SelectEmployer(this.EmployerEntry);
        }


        public void CommitChanges()
        {
            this.Presenter.DeleteNewEmployer(this.EmployerEntry.Employer);
        }

        #endregion
    }
}