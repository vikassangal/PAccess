using System;
using System.ComponentModel;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// This interface will be used and fully populated when all non UI related logic is moved from the <see cref="PatientsAccountsView"/> to the 
    /// <see cref="PatientAccountsViewPresenter"/>. The inheritance from <see cref="IPatientsAccountsPresenter"/> will be removed when the view refactoring is completed. 
    /// </summary>
    public interface IPatientsAccountsView : IPatientsAccountsPresenter, IDisposable
    {

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        void AfterWork( object sender, RunWorkerCompletedEventArgs e );
        /// <summary>
        /// Enables the buttons for activity. This method currently forwards
        /// calls to the presenter. When the migration to the passive view
        /// pattern is complete for this view it can be removed.
        /// </summary>
        void EnableButtonsForActivity();

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        void Show();

        object Model { get; }
    }
}
