using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.NewEmployersManagement
{
    internal sealed class MoveEmployerToMasterListCommand : ICommand
    {
        private NewEmployerEntry _employerEntry;
        private NewEmployersManagementPresenter _presenter;
        private ContactPoint _contactPointToMove;


        public MoveEmployerToMasterListCommand(NewEmployersManagementPresenter presenter, NewEmployerEntry employer)
        {
            this.Presenter = presenter;
            this.EmployerEntry = employer;
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

        private NewEmployersManagementPresenter Presenter
        {
            get
            {
                return this._presenter;
            }
            set
            {
                this._presenter = value;
            }
        }

        #region ICommand Members

        public void ExecuteUIAction()
        {
            this.Presenter.MasterListEmployers.Clear();
            this._contactPointToMove = EmployerHelper.GetFirstContactPointFor(this.EmployerEntry.Employer);
            this._contactPointToMove.PhoneNumber = new PhoneNumber(Presenter.NewEmployerListView.SelectedEmployerPhoneNumber);
            this.Presenter.MasterListEmployers.Add(this.EmployerEntry.Employer);
            this.EmployerEntry.Employer.Name = Presenter.NewEmployerListView.SelectedEmployerName;
            this.EmployerEntry.Employer.NationalId = Presenter.NewEmployerListView.SelectedEmployerNationalId;
            this.Presenter.NewEmployers.Remove(this.EmployerEntry);
            this.Presenter.MasterEmployerListView.SearchString = string.Empty;
            this.Presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            this.Presenter.UpdateNewEmployersViewWithoutAnySelection();
        }


        public void UndoUIAction()
        {
            this.Presenter.MasterListEmployers.Clear();
            this.Presenter.NewEmployers.Add(this.EmployerEntry);
            this.Presenter.UpdateMasterEmployerViewWithFirstItemSelected();
            this.Presenter.UpdateNewEmployersViewWithoutAnySelection();
            this.Presenter.SelectNewEmployerAndSearchMasterEmployerList(this.EmployerEntry);
            this.Presenter.NewEmployerListView.SelectEmployer(this.EmployerEntry);
        }


        public void CommitChanges()
        {
            this.Presenter.DeleteNewEmployer(this.EmployerEntry.Employer);
            this.Presenter.SaveEmployerToMasterList(this.EmployerEntry);
        }

        #endregion
    }
}