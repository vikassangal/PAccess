using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews.Views
{
    public interface IAdditionalRacesView
    {
        void PopulateRace(ArrayList raceArrayList);
        Race Race3 { get; set; }
        Race Race4 { get; set; }
        Race Race5 { get; set; }
        Account ModelAccount { get; set; }
        void EnableRace4ComboBox();
        void DisableRace4ComboBox();
        void EnableRace5ComboBox();
        void DisableRace5ComboBox();
        void ShowAdditionalRacesView();
        void SetRace3DeactivatedBgColor();
        void ProcessRace3InvalidCodeEvent();
        void SetRace4DeactivatedBgColor();
        void ProcessRace4InvalidCodeEvent();
        void SetRace5DeactivatedBgColor();
        void ProcessRace5InvalidCodeEvent();
        IAdditionalRacesViewPresenter AdditionalRacesViewPresenter { set; }
    }
}
