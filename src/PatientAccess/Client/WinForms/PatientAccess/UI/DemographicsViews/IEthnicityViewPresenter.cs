
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IEthnicityViewPresenter
    {
        void UpdateView();
        void RunInvalidCodeRules();
        void RunRules();
        void UpdateEthnicityAndDescentModelValue(Ethnicity ethnicity);

    }
}
