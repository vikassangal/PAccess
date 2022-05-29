using System; 
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IPatientNameView
    {
        Account ModelAccount { get; set; }
        String PatientFirstName { get; set; }
        Gender PatientGender { get; set; }
        PatientNamePresenter PatientNamePresenter { get; set; }
        void RegisterPatientNameRequiredEvent();
    }
}
