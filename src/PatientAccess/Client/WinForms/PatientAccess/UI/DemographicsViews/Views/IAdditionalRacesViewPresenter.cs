using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews.Views
{
   public interface IAdditionalRacesViewPresenter
    {
        void UpdateView();
        void RunInvalidCodeRules();
        void UpdateRace3ToModel();
        void UpdateRace4ToModel();
        void UpdateRace5ToModel();
        void ShowAdditionalRacesView();
        bool IsValidRace3 { get; }
        bool IsValidRace4 { get; }
        bool ShouldAdditionRaceEditButtonBeVisible(Account account);
        bool ShouldAdditionRaceEditButtonBeEnabled(Account account);

    }
}
