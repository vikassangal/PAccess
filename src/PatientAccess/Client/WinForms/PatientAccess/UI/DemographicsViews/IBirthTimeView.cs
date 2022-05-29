using System;
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IBirthTimeView
    {
        Account ModelAccount { get; set; }
        void ShowBirthTimeEnabled();
        void ShowBirthTimeDisabled();
        void DisableAndHideBirthTime();
        void PopulateBirthTime( DateTime birthDate );
        void MakeBirthTimeRequired();
        void MakeBirthTimePreferred();
    }
}
