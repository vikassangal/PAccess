
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IRaceViewPresenter
    {
        void UpdateView();
        void RunInvalidCodeRules();
        void RunRules();
        void UpdateRaceAndNationalityModelValue(Race race);
        

    }
}
