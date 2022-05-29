using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.NewEmployersManagement
{
    public interface INewEmployersListView
    {
        string SelectedEmployerName { get; set; }

        string SelectedEmployerNationalId { get; set; }

        string SelectedEmployerAddress { get; set; }

        string SelectedEmployerPhoneNumber { get; set; }

        INewEmployersManagementPresenter Presenter { get; set; }

        bool EditAddressEnabled { get; set; }

        bool ClearEmployerContactInformationEnabled { get; set; }

        bool MoveAllInfoEnabled { get; set; }

        bool MoveAddressAndPhoneEnabled { get; set; }

        bool DeleteEnabled { get; set; }

        bool UndoEnabled { get; set; }

        bool SelectedEmployerNameFieldEnabled { get; set; }

        bool SelectedEmployerNationalIdFieldEnabled { get; set; }

        bool SelectedEmployerPhoneFieldEnabled { get; set; }

        bool FinishEnabled { get; set; }

        bool CancelEnabled { get; set; }
        
        bool SelectedEmployerAddressFieldEnabled { get; set; }

        void SelectEmployer(NewEmployerEntry employerEntry);

        /// <summary>
        /// Updates the view with the new employers without selecting any employer.
        /// </summary>
        /// <param name="employers">The employers.</param>
        void ShowEmployersWithoutSelection(IEnumerable<NewEmployerEntry> employers);

        void ShowNoNewUsersAvailableMessage();
        
        /// <summary>
        /// Shows the retrieving new employers message.
        /// </summary>
        void ShowRetrievingNewEmployersMessage();

        void ClearRetrievingNewEmployersMessage();

        void ShowSavingMessage();

        void ClearSavingMessage();

        void ShowFeatureIsLockedMessage();

        void DisplayMoveAllInfoMessageWithNoEmployerSelection();

        void DisplayMoveAllInfoMessageWithEmployerSelection();
        
        void UpdateEmployer(NewEmployerEntry employerEntry);
    }
}