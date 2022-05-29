using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.NewEmployersManagement
{
    internal sealed class MoveAddressAndPhoneToMasterListCommand : ICommand
    {
        private NewEmployerEntry _employer;
        private Employer _masterListEmployer;
        private NewEmployersManagementPresenter _presenter;
        private ContactPoint _contactPointToMove;


        public MoveAddressAndPhoneToMasterListCommand(NewEmployersManagementPresenter presenter,
                                                       NewEmployerEntry employer,
                                                       Employer masterListEmployer)
        {
            this.Presenter = presenter;
            this.NewEmployerEntry = employer;
            this.MasterListEmployer = masterListEmployer;
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

        private NewEmployerEntry NewEmployerEntry
        {
            get
            {
                return _employer;
            }

            set
            {
                _employer = value;
            }
        }

        private Employer MasterListEmployer
        {
            get
            {
                return _masterListEmployer;
            }
            set
            {
                _masterListEmployer = value;
            }
        }

        #region ICommand Members

        public void ExecuteUIAction()
        {
            this.Presenter.NewEmployers.Remove(this.NewEmployerEntry);
            this._contactPointToMove = EmployerHelper.GetFirstContactPointFor(this.NewEmployerEntry.Employer);
            this._contactPointToMove.PhoneNumber = new PhoneNumber(Presenter.NewEmployerListView.SelectedEmployerPhoneNumber);
            
            this.AddContactPointToEmployerAtFirstPosition();
            
            this.Presenter.UpdateNewEmployersViewWithoutAnySelection();
            this.Presenter.UpdateMasterEmployerViewWithoutSelection();
            this.Presenter.SelectEmployerOnMasterEmployerView(this.MasterListEmployer);
            this.Presenter.MasterEmployerListView.SelectFirstAddress();
        }


        private void AddContactPointToEmployerAtFirstPosition() 
        {
            ICollection existingContactPoints = this.MasterListEmployer.ContactPoints;
            List<ContactPoint> contactPoints = new List<ContactPoint>();
            contactPoints.Add(this._contactPointToMove);
         
            foreach (object contactPoint in existingContactPoints)
            {
                this.MasterListEmployer.RemoveContactPoint((ContactPoint)contactPoint);
                contactPoints.Add((ContactPoint)contactPoint);
            }

            foreach (ContactPoint contactPoint in contactPoints)
            {
                this.MasterListEmployer.AddContactPoint(contactPoint);
            }
        }


        public void UndoUIAction()
        {
            this.Presenter.NewEmployers.Add(this.NewEmployerEntry);
            this.MasterListEmployer.RemoveContactPoint(this._contactPointToMove);
            this.Presenter.UpdateNewEmployersViewWithoutAnySelection();
            this.Presenter.UpdateMasterEmployerViewWithoutSelection();
            this.Presenter.SelectEmployerOnMasterEmployerView(this.MasterListEmployer);
            this.Presenter.NewEmployerListView.SelectEmployer(this.NewEmployerEntry);
        }


        public void CommitChanges()
        {
            this.Presenter.DeleteNewEmployer(this.NewEmployerEntry.Employer);
            this.Presenter.SaveContactPointForMasterListEmployer(this.MasterListEmployer, this._contactPointToMove);
        }

        #endregion
    }
}