using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.NewEmployersManagement
{
    ///<summary>
    ///
    ///</summary>
    public interface INewEmployersManagementPresenter
    {
        /// <summary>
        /// Gets or sets the new employer list view.
        /// </summary>
        /// <value>The new employer list view.</value>
        INewEmployersListView NewEmployerListView { get; set; }

        /// <summary>
        /// Gets or sets the master employer list view.
        /// </summary>
        /// <value>The master employer list view.</value>
        IMasterEmployersListView MasterEmployerListView { get; set; }

        /// <summary>
        /// Gets the selected new employer.
        /// </summary>
        /// <value>The selected new employer.</value>
        NewEmployerEntry SelectedNewEmployerEntry { get; set; }

        /// <summary>
        /// Deletes the new employer.
        /// </summary>
        void DeleteSelectedNewEmployer();

        void SelectEmployerOnMasterEmployerView(Employer employer);

        void AddNewEmployer(NewEmployerEntry employer);

        string GetFormattedEmployerInfo(NewEmployerEntry employer);

        /// <summary>
        /// Clears the address and phone number.
        /// </summary>
        void ClearAddressAndPhoneNumber();


        /// <summary>
        /// Moves the new employer to the master list.
        /// </summary>
        void MoveSelectedNewEmployerToMasterList();


        /// <summary>
        /// Moves the address and phone to the master list.
        /// </summary>
        void MoveAddressAndPhoneToMasterList();


        /// <summary>
        /// Performs the Undo operation.
        /// </summary>
        void Undo();


        /// <summary>
        /// Saves all the changes.
        /// </summary>
        void Save();

        void SearchForEmployerInMasterList(string searchText);

        bool IsPhoneNumberValid(string phoneNumber);

        void UpdateNewEmployersViewWithoutAnySelection();

        void SelectNewEmployerAndSearchMasterEmployerList(NewEmployerEntry employerEntry);

        //void RefreshNewEmployerList();

        void ShowFeatureIsLockedMessage();

        void SearchStringChanged();

        bool IsEmployerNameValid(string employerName);
        
        void SelectedEmployerNameChanged();
        
        void SelectedEmployerNationalIdChanged();
        void SelectedEmployerPhoneChanged();
        void UpdateSelectedNewEmployerOnTheView();
		void ChangeSelectedNewEmployerAddress( Address newlySelectedAddress );
        
        bool IsSearchStringValid(string searchString);
    }
}