using System.Collections.Generic;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.NewEmployersManagement
{
    ///<summary>
    ///
    ///</summary>
    public interface IMasterEmployersListView
    {
        /// <summary>
        /// Gets or sets the search string used for searching the master employer list.
        /// </summary>
        /// <value>The search string.</value>
        string SearchString { get; set; }

        bool IsSearchButtonEnabled { get; set; }

        void ShowEmployersWithoutSelection(IEnumerable<Employer> employers);

        void ShowMessageWhenSearchReturnsNoResults();

        void ShowSelectedEmployerAddresses(IEnumerable<string> addresses);

        void ShowMessageWhenEmployerDoesNotHaveAnAddress();

        void SelectEmployer(Employer employer);

        void ShowSearchInProgressMessage();

        void ClearSearchInProgressMessage();
        
        void ClearMasterListViewResults();
        
        void SelectFirstAddress();
    }
}