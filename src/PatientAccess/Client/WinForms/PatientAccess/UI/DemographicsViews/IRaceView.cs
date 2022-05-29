using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IRaceView
    {
        void PopulateRace(IDictionary<Race, ArrayList> raceNationalityDictionary);
        Race Race { get; set; }
        Account ModelAccount { get; set; }
        void SetDeactivatedBgColor();
        void ProcessInvalidCodeEvent();
        void SetRaceAsRequiredColor();
        void SetRaceAsPreferredColor();
        void SetNormalBgColor();
        IRaceViewPresenter RaceViewPresenter { get; set; }
        void SetSizeForRaceDropdownButton();
       
    }
}
